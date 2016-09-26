﻿
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using LitJson;

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
                else
                {
                    string[] values = record.Split(new string[] { "\t" }, StringSplitOptions.None);
                    Debug.Log("values -> " + values);

                    if (values.Length != headerMapping.Count)
                    {
                        continue;
                    }

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
                                else if (fieldType == typeof(Enum))
                                {
                                    //field.SetValue(deploy, val != "0" ? true : false);
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
                                    //field.SetValue(deploy, LitJson.JsonMapper.ToObject(val));
                                }
                                else if (fieldType == typeof(object))
                                {
                                    //field.SetValue(deploy, LitJson.JsonMapper.ToObject(fieldType, val));
                                    Debug.Log("===========");
                                }

                                Debug.Log("FieldType = " + fieldType);
                            }
                        }
                    }

                    if (deploy != null)
                    {
                        ProtocolDeploy protocolDeploy = (ProtocolDeploy)(object)deploy;
                        Debug.Log(JsonMapper.ToJson(protocolDeploy));
                    }
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
