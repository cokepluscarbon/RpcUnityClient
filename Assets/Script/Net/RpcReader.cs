
using System;
using System.IO;
using System.Collections;

public class RpcReader
{
    private BinaryReader reader;

    public RpcReader(byte[] bytes)
    {
        reader = new BinaryReader(new MemoryStream(bytes));
    }

    public T Read<T>()
    {
        Type type = typeof(T);

        if (type == typeof(byte))
        {
            return (T)(object)reader.ReadByte();
        }
        else if (type == typeof(bool))
        {
            return (T)(object)reader.ReadBoolean();
        }
        else if (type == typeof(short))
        {
            return (T)(object)reader.ReadInt16();
        }
        else if (type == typeof(int))
        {
            return (T)(object)reader.ReadInt32();
        }
        else if (type == typeof(long))
        {
            return (T)(object)reader.ReadInt64();
        }
        else if (type == typeof(float))
        {
            return (T)(object)reader.ReadDecimal();
        }
        else if (type == typeof(double))
        {
            return (T)(object)reader.ReadDecimal();
        }
        else if (type == typeof(string))
        {
            int len = Read<int>();
            byte[] bytes = new byte[len];

            return (T)(object)System.Text.Encoding.UTF8.GetString(bytes);
        }
        else if (type == typeof(Object))
        {
            string json = Read<string>();

            return LitJson.JsonMapper.ToObject<T>(json);
        }

        return default(T);
    }

}
