
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

public class TableLoader
{

    public static T Load<T>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("config/table/protocol");

        string texts = textAsset.text;
        string[] records = texts.Split(new string[] { "\r\n" }, StringSplitOptions.None);

        Dictionary<string, int> headerMapping = null;
        foreach (string record in records)
        {
            if (!record.StartsWith("#"))
            {
                if (headerMapping == null)
                {
                    headerMapping = GetHeaderMapping(record);
                }
                else {
                    string[] values = record.Split(new string[] { "\t" }, StringSplitOptions.None);
                    Debug.Log("values -> " + values);

                    FieldInfo[] fields = typeof(T).GetFields();
                    T deploy = Activator.CreateInstance<T>();
                    foreach (FieldInfo field in fields)
                    {
                        Debug.Log(field.Name);
                        string name = field.Name;
                        if (headerMapping.ContainsKey(name))
                        {
                            int index = headerMapping[name];
                            if (values.Length > index) {
                                string val = values[index];
                                Type fieldType = field.FieldType;
                                Debug.Log(fieldType);
                                if (fieldType == typeof(int))
                                {
                                    field.SetValue(deploy, int.Parse(val));
                                }
                                else if (fieldType == typeof(string))
                                {
                                    field.SetValue(deploy, val);
                                }
                            }
                        }
                    }

                    ProtocolDeploy protocolDeploy = (ProtocolDeploy)(object)deploy;
                    Debug.Log("---> " + protocolDeploy.id + ":" + protocolDeploy.rpc);
                }
            }
        }
        return default(T);
    }

    private static Dictionary<string, int> GetHeaderMapping(string record)
    {
        string[] headers = record.Split(new string[] { "\t" }, StringSplitOptions.None);

        Dictionary<string, int> mapping = new Dictionary<string, int>();
        for (int i = 0; i < headers.Length; i++)
        {
            mapping.Add(headers[i], i);
        }

        return mapping;
    }

}
