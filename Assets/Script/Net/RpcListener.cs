using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class RpcListener : Attribute
{
    private string rpc;
    public RpcListener(string rpc)
    {
        this.rpc = rpc;
    }
}