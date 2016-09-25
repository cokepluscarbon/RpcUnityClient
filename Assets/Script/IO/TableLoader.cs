
using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TableLoader
{

    public static T Load<T>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("config/table/protocol");

        string texts = textAsset.text;
        string[] records = texts.Split(new string[] { "\r\n" }, StringSplitOptions.None);

        string[] headers = null;
        foreach (string record in records)
        {
            if (!record.StartsWith("#"))
            {
                if (headers == null)
                {
                    headers = ParseHeader(record);
                }
                else {
                    string[] values = record.Split(new string[] { "\t" }, StringSplitOptions.None);
                }
            }
            Debug.Log("-> " + record);
        }
        return default(T);
    }

    private static string[] ParseHeader(string record)
    {
        return record.Split(new string[] { "\t" }, StringSplitOptions.None);
    }

}
