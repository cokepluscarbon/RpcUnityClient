using UnityEngine;
using System.Collections;

public class BaseDeploy
{
    public int id;

    public static implicit operator bool(BaseDeploy deploy)
    {
        return deploy != null;
    }
}