using System;
using System.Reflection;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class Deploy : Attribute
{
    private string path;
    public Deploy(string path)
    {
        this.path = path;
    }

    public static T GetDeploy<T>(int id) where T : BaseDeploy
    {
        Type type = typeof(T);

        object[] attributes = type.GetCustomAttributes(false);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is Deploy)
            {
                Deploy deploy = (Deploy)attributes[i];
                return TableLoader.Load<T>(deploy.path, id);
            }
        }

        return null;
    }
}