
using System;
using System.IO;
using System.Net;
using System.Collections;

public class RpcReader
{
    private BinaryReader reader;

    public RpcReader(byte[] bytes)
    {
        reader = new BinaryReader(new MemoryStream(bytes));
    }

    public byte ReadByte()
    {
        return reader.ReadByte();
    }

    public short ReadShort()
    {
        return IPAddress.NetworkToHostOrder(reader.ReadInt16());
    }

    public int ReadInt()
    {
        return IPAddress.NetworkToHostOrder(reader.ReadInt32());
    }

    public long ReadLong()
    {
        return IPAddress.NetworkToHostOrder(reader.ReadInt64());
    }

    public float ReadFloat()
    {
        return reader.ReadSingle();
    }

    public double ReadDouble()
    {
        return reader.ReadDouble();
    }

    public byte[] ReadBytes()
    {
        short len = ReadShort();
        return reader.ReadBytes(len);
    }

    public string ReadString()
    {
        byte[] bytes = ReadBytes();
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    public object ReadObject<T>()
    {
        string json = ReadString();
        return LitJson.JsonMapper.ToObject<T>(json);
    }

}
