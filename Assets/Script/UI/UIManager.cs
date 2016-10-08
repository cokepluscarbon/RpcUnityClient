
using System;
using UnityEngine;
using System.Collections.Generic;

public class UIManager
{
    private static GameObject _canvasGo;
    private static GameObject canvasGo { get { return _canvasGo ?? Init(); } set { _canvasGo = value; } }
    private static Dictionary<string, List<Action<UIBaseWidget>>> loadingDict = new Dictionary<string, List<Action<UIBaseWidget>>>();
    private static Stack<ViewNode> saveViewNodeStack = new Stack<ViewNode>();
    private static Stack<ViewNode> viewNodeStack = new Stack<ViewNode>();

    private static GameObject Init()
    {
        _canvasGo = GameObject.Find("Canvas");
        if (_canvasGo != null)
        {
            UnityEngine.Object.DontDestroyOnLoad(_canvasGo);

            foreach (UIViewLayer uiViewLayer in Enum.GetValues(typeof(UIViewLayer)))
            {
                if (uiViewLayer != UIViewLayer.None)
                {
                    GameObject viewLayerGo = new GameObject();
                    RectTransform viewLayerRect = viewLayerGo.AddComponent<RectTransform>();

                    viewLayerGo.name = uiViewLayer.ToString();
                    viewLayerRect.SetParent(_canvasGo.transform);
                    ResetUIAnchor(viewLayerRect);
                }
            }
        }

        return _canvasGo;
    }

    public static void ShowView(int id)
    {
        LoadView(id, null, true);
    }


    public static void ShowView(int id, Action<UIBaseView> calllback)
    {
        LoadView(id, calllback, true);
    }


    public static void ShowView(int id, Action<UIBaseView> calllback, params object[] args)
    {
        LoadView(id, calllback, true, args);
    }

    public static void LoadWidget(string viewPath, UIViewLayer viewLayer, Type type, Action<UIBaseWidget> callback = null)
    {
        LoadResourceToViewLayer(viewPath, viewLayer, obj =>
        {
            Component component = obj.AddComponent(type);
            if (callback != null)
            {
                callback(component.GetComponent<UIBaseWidget>());
            }
        });
    }

    public static void LoadWidgetSingleton(string viewPath, UIViewLayer viewLayer, string typeStr, Action<UIBaseWidget> callback = null)
    {
        Type type = Type.GetType(typeStr);
        LoadWidgetSingleton(viewPath, viewLayer, type, callback);
    }


    public static void LoadWidgetSingleton(string viewPath, UIViewLayer viewLayer, Type type, Action<UIBaseWidget> callback = null)
    {
        UIBaseWidget targetWidget = FindWidgetInLayers(viewLayer, type);
        if (targetWidget)
        {
            if (callback != null)
            {
                callback(targetWidget);
            }
        }
        else
        {
            List<Action<UIBaseWidget>> callbackList = null;
            if (loadingDict.TryGetValue(viewPath, out callbackList) && callbackList.Count > 0)
            {
                loadingDict[viewPath].Add(callback);
            }
            else
            {
                callbackList = callbackList ?? new List<Action<UIBaseWidget>>();
                callbackList.Add(callback);
                loadingDict.Add(viewPath, callbackList);
                LoadWidget(viewPath, viewLayer, type, uiBaseWidget =>
                {
                    foreach (Action<UIBaseWidget> cb in callbackList)
                    {
                        if (cb != null)
                        {
                            cb(uiBaseWidget);
                        }
                    }
                    loadingDict.Remove(viewPath);
                });
            }
        }
    }

    public static void BackView(Action<UIBaseView> callback = null)
    {
        if (viewNodeStack.Count > 1)
        {
            ViewNode destroyViewNode = viewNodeStack.Pop();
            ViewNode.DestroyView(destroyViewNode);

            ViewNode topViewNode = viewNodeStack.Pop();
            topViewNode.ShowAsTopView(callback);

            ViewNode secondViewNode = null;
            if (viewNodeStack.Count > 0)
            {
                secondViewNode = viewNodeStack.Pop();

                secondViewNode.ShowAsSecondView(topViewNode.uiViewDeploy.uiViewLayer);
                viewNodeStack.Push(secondViewNode);
            }

            viewNodeStack.Push(topViewNode);

            if (secondViewNode != null && secondViewNode.CheckInViewLayer(UIViewLayer.View))
            {
                ShowWidgetInToolbarLayer(secondViewNode);
            }
            if (topViewNode.CheckInViewLayer(UIViewLayer.View))
            {
                ShowWidgetInToolbarLayer(topViewNode);
            }
            if (destroyViewNode.CheckInViewLayer(UIViewLayer.FullView))
            {
                topViewNode.ShowEffects(true);
            }
        }
    }

    private static void ShowMainViews(Action<UIBaseView> callback = null)
    {
        if (saveViewNodeStack != null)
        {
            viewNodeStack = saveViewNodeStack;
        }

        if (viewNodeStack.Count > 1)
        {
            BackView(callback);
        }
        else
        {
            ShowView(11, callback);
        }
    }

    public static void ClearViews(bool saveStack = false)
    {
        if (saveStack)
        {
            viewNodeStack.Push(ViewNode.CreateVirtualViewNode());
            saveViewNodeStack = viewNodeStack;
        }
        viewNodeStack = new Stack<ViewNode>();

        foreach (UIViewLayer uiViewLayer in Enum.GetValues(typeof(UIViewLayer)))
        {
            if (uiViewLayer != UIViewLayer.System)
            {
                Transform viewLayerTransf = canvasGo.transform.Find(uiViewLayer.ToString());
                if (viewLayerTransf != null)
                {
                    for (int i = 0; i < viewLayerTransf.transform.childCount; i++)
                    {
                        Transform childTrans = viewLayerTransf.transform.GetChild(i);
                        UIBase uiBase = childTrans.GetComponent<UIBase>();
                        if (!uiBase || !uiBase.neverDestory)
                        {
                            GameObject.Destroy(viewLayerTransf.transform.GetChild(i).gameObject);
                        }
                    }
                }
            }
        }
    }

    private static void HidePreViews()
    {
        if (viewNodeStack.Count >= 1)
        {
            foreach (ViewNode viewNode in viewNodeStack)
            {
                if (viewNode.CheckViewExist())
                {
                    viewNode.uiView.gameObject.SetActive(false);
                }
            }
        }
    }

    private static int preLoadId;
    private static void LoadView(int id, Action<UIBaseView> callback, bool pushStack, params object[] args)
    {
        var canvas = canvasGo;
        if (id == preLoadId || id == 0)
        {
            return;
        }

        if (id == -1)
        {
            ShowMainViews(callback);
            return;
        }

        UIViewDeploy viewDeploy = Deploy.GetDeploy<UIViewDeploy>(1);
        if (viewDeploy)
        {
            ViewNode preViewNode = viewNodeStack.Count > 0 ? viewNodeStack.Peek() : null;
            ViewNode curViewNode = new ViewNode(null, viewDeploy, args);
            LoadRawView(viewDeploy, uiBaseView =>
            {
                if (viewDeploy.uiViewLayer == UIViewLayer.View)
                {
                    HidePreViews();
                }
                else
                {
                    viewNodeStack.Peek().ShowEffects(false);
                }

                uiBaseView.args = args;
                curViewNode.uiView = uiBaseView;
                curViewNode.SetAsLastSibling(true);

                if (pushStack)
                {
                    viewNodeStack.Push(curViewNode);
                }

                if (curViewNode.CheckInViewLayer(UIViewLayer.View))
                {
                    ShowWidgetInToolbarLayer(curViewNode);
                }
                else
                {
                    ShowWidgetInToolbarLayer(preViewNode);
                }

                if (callback != null)
                {
                    callback(uiBaseView);
                }

                preLoadId = 0;
            });
        }
        else
        {
            Debug.LogErrorFormat("UIViewDeploy[id={0}] not found!", id);
        }
    }

    private static void LoadRawView(UIViewDeploy viewDeploy, Action<UIBaseView> callback)
    {
        string viewPath = viewDeploy.viewPath;

        LoadResourceToViewLayer(viewPath, viewDeploy.uiViewLayer, gameObject =>
        {
            UIBaseView uiBaseView = null;
            if (!string.IsNullOrEmpty(viewDeploy.entityClass))
            {
                Type entityClass = Type.GetType(viewDeploy.entityClass);
                uiBaseView = gameObject.AddComponent(entityClass).gameObject.GetComponent<UIBaseView>();
            }

            if (callback != null)
            {
                callback(uiBaseView);
            }
        });
    }

    private static UIBaseWidget FindWidgetInLayers(UIViewLayer viewLayer, Type type)
    {
        Transform layerRect = canvasGo.transform.Find(viewLayer.ToString());
        if (layerRect)
        {
            for (int i = 0; i < layerRect.childCount; i++)
            {
                Transform childRect = layerRect.GetChild(i);
                Component component = childRect.GetComponent(type);
                if (component)
                {
                    return component.GetComponent<UIBaseWidget>();
                }
            }
        }

        return null;

    }

    private static void LoadResourceToViewLayer(string viewPath, UIViewLayer viewLayer, Action<GameObject> callback)
    {
        ResourcesDatabase.Load(viewPath, _object =>
        {
            GameObject viewGo = GameObject.Instantiate(_object) as GameObject;
            RectTransform rectTrans = viewGo.GetComponent<RectTransform>();

            rectTrans.SetParent(canvasGo.transform.Find(viewLayer.ToString()), false);

            callback(viewGo);
        });
    }

    private static void ShowWidgetInToolbarLayer(ViewNode viewNode)
    {
        //UIBackButton.Show(viewNode != null && viewNode.ShowBackButton());
        //UIToolbar.Show(viewNode != null && viewNode.ShowToolbar());
    }

    private static void ResetUIAnchor(RectTransform viewLayerRect)
    {
        viewLayerRect.localScale = Vector3.one;
        viewLayerRect.localPosition = Vector3.zero;
        viewLayerRect.anchorMin = Vector2.zero;
        viewLayerRect.anchorMax = Vector2.one;
        viewLayerRect.gameObject.layer = 5;
        viewLayerRect.offsetMin = Vector2.zero;
        viewLayerRect.offsetMax = Vector2.zero;
    }

    private class ViewNode
    {
        public UIBaseView uiView;
        public UIViewDeploy uiViewDeploy;
        public object[] args;

        public ViewNode(UIBaseView uiView, UIViewDeploy uiViewDeploy, object[] args)
        {
            this.uiView = uiView;
            this.uiViewDeploy = uiViewDeploy;
            this.args = args;
        }

        public bool CheckViewExist()
        {
            return uiView && uiView.gameObject;
        }

        public bool CheckInViewLayer(UIViewLayer viewLayer)
        {
            return uiViewDeploy != null && uiViewDeploy.uiViewLayer == viewLayer;
        }

        public void SetAsLastSibling(bool show)
        {
            if (CheckViewExist())
            {
                uiView.gameObject.SetActive(show);
                uiView.transform.SetAsLastSibling();
            }
        }

        public void SetAsSiblingIndex(bool show, int index)
        {
            if (CheckViewExist())
            {
                uiView.gameObject.SetActive(show);
                uiView.transform.SetSiblingIndex(index);
            }
        }

        public void ShowAsTopView(Action<UIBaseView> callback = null)
        {
            if (CheckViewExist())
            {
                SetAsLastSibling(true);
                if (callback != null)
                {
                    callback(uiView);
                }
            }
            else
            {
                LoadRawView(uiViewDeploy, _uiView =>
                {
                    uiView = _uiView;
                    uiView.args = args;
                    SetAsLastSibling(true);
                    if (callback != null)
                    {
                        callback(uiView);
                    }
                });
            }
        }

        public void ShowAsSecondView(UIViewLayer currViewLayer)
        {
            if (CheckViewExist())
            {
                ShowAsSecondViewImpl(currViewLayer);
            }
            else
            {
                LoadRawView(uiViewDeploy, _uiView =>
                {
                    uiView = _uiView;
                    uiView.args = args;
                    ShowAsSecondViewImpl(currViewLayer);
                });
            }
        }

        public static void DestroyView(ViewNode viewNode)
        {
            if (viewNode.CheckViewExist())
            {
                GameObject.Destroy(viewNode.uiView.gameObject);
            }
        }

        public static ViewNode CreateVirtualViewNode()
        {
            GameObject virtualViewNode = new GameObject();
            UIBaseView virtualView = virtualViewNode.AddComponent<UIBaseView>();

            return new ViewNode(virtualView, null, null);
        }

        private void ShowAsSecondViewImpl(UIViewLayer currViewLayer)
        {
            bool checkViewShow = currViewLayer == UIViewLayer.FullView;
            SetAsSiblingIndex(checkViewShow, viewNodeStack.Count);
        }

        public void ShowEffects(bool show)
        {
            if (CheckViewExist())
            {
                ParticleSystem[] particleSystems = uiView.transform.GetComponentsInChildren<ParticleSystem>();
                if (particleSystems != null)
                {
                    foreach (ParticleSystem particleSystem in particleSystems)
                    {
                        if (show)
                        {
                            particleSystem.Play();
                        }
                        else
                        {
                            particleSystem.Stop();
                        }
                    }
                }

            }
        }

        public bool ShowBackButton()
        {
            return CheckInViewLayer(UIViewLayer.View) && uiViewDeploy.showBack;
        }

        public bool ShowToolbar()
        {
            return CheckInViewLayer(UIViewLayer.View) && uiViewDeploy.showToolbar;
        }
    }

}
