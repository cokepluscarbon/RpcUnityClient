using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class Deploy : Attribute
{
    private string path;
    public Deploy(string path)
    {
        this.path = path;
    }
}