
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using LitJson;

public class TableLoader
{
    private static Dictionary<string, Dictionary<int, BaseDeploy>> tableDict = new Dictionary<string, Dictionary<int, BaseDeploy>>();

    public static T Load<T>(string path, int id) where T : BaseDeploy
    {
        if (!tableDict.ContainsKey(path))
        {
            Dictionary<int, BaseDeploy> table = LoadTable<T>(path);
            tableDict.Add(path, table);
        }

        if (tableDict.ContainsKey(path))
        {
            if (tableDict[path][id])
            {
                return (T)tableDict[path][id];
            }
        }

        return null;
    }

    private static Dictionary<int, BaseDeploy> LoadTable<T>(string path) where T : BaseDeploy
    {
        Dictionary<int, BaseDeploy> tDict = new Dictionary<int, BaseDeploy>();

        Dictionary<string, int> headerMapping = null;
        string[] records = LoadTableRecords(path);
        foreach (string record in records)
        {
            if (!record.StartsWith("#"))
            {
                if (headerMapping == null)
                {
                    headerMapping = GetHeaderMapping(record);
                }
                else
                {
                    string[] values = record.Split(new string[] { "\t" }, StringSplitOptions.None);
                    if (values.Length == headerMapping.Count)
                    {
                        T deploy = ParseDeploy<T>(headerMapping, values);
                        tDict.Add(deploy.id, deploy);
                    }
                }
            }
        }


        return tDict;
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

    private static T ParseDeploy<T>(Dictionary<string, int> headerMapping, string[] values) where T : BaseDeploy
    {
        FieldInfo[] fields = typeof(T).GetFields();
        T deploy = Activator.CreateInstance<T>();
        foreach (FieldInfo field in fields)
        {
            string name = field.Name;

            if (headerMapping.ContainsKey(name))
            {
                int index = headerMapping[name];
                if (values.Length > index)
                {
                    string val = values[index].Trim();
                    Type fieldType = field.FieldType;

                    Debug.Log(fieldType);
                    if (fieldType == typeof(byte))
                    {
                        field.SetValue(deploy, byte.Parse(val));
                    }
                    else if (fieldType == typeof(int))
                    {
                        field.SetValue(deploy, int.Parse(val));
                    }
                    else if (fieldType == typeof(short))
                    {
                        field.SetValue(deploy, short.Parse(val));
                    }
                    else if (fieldType == typeof(long))
                    {
                        field.SetValue(deploy, long.Parse(val));
                    }
                    else if (fieldType == typeof(double))
                    {
                        field.SetValue(deploy, double.Parse(val));
                    }
                    else if (fieldType == typeof(string))
                    {
                        field.SetValue(deploy, val);
                    }
                    else if (fieldType == typeof(Enum))
                    {
                        field.SetValue(deploy, Enum.Parse(fieldType, val));
                    }
                    else if (fieldType == typeof(List<bool>))
                    {
                        field.SetValue(deploy, LitJson.JsonMapper.ToObject<List<bool>>(val));
                    }
                    else if (fieldType == typeof(List<int>))
                    {
                        field.SetValue(deploy, LitJson.JsonMapper.ToObject<List<int>>(val));
                    }
                    else if (fieldType == typeof(List<long>))
                    {
                        field.SetValue(deploy, LitJson.JsonMapper.ToObject<List<long>>(val));
                    }
                    else if (fieldType == typeof(List<string>))
                    {
                        field.SetValue(deploy, LitJson.JsonMapper.ToObject<List<string>>(val));
                    }
                    else if (fieldType == typeof(JsonData))
                    {
                    }
                    else if (fieldType == typeof(object))
                    {
                    }
                }
            }
        }

        return deploy;
    }

    private static string[] LoadTableRecords(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("config/table/" + path);

        string texts = textAsset.text;
        string[] records = texts.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        return records;
    }
}
