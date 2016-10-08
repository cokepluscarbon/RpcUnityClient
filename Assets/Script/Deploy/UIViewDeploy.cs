using UnityEngine;
using System.Collections;

[Deploy("uiView")]
public class UIViewDeploy : BaseDeploy
{
    public int id;
    public string viewPath;
    public string entityClass;
    public string luaTable;
    public UIViewLayer uiViewLayer;
    public bool showBack;
    public bool showToolbar;

}
