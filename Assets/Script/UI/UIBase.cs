using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIBase : UIBehaviour
{
    public bool neverDestory = false;

    public T Find<T>(string path)
    {
        Transform transf = transform.Find(path);
        if (transf == null)
        {
            Debug.LogError(string.Format("UI[class={0}] could not find path[{1}]", GetType(), path));
            return default(T);
        }

        return transf.GetComponent<T>();
    }

    public Transform Find(string path)
    {
        return transform.Find(path);
    }

}
