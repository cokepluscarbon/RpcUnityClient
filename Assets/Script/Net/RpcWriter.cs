
using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Net;

public class RpcWriter
{
    private BinaryWriter writer;
    private MemoryStream stream;

    public RpcWriter()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    public void Write(byte value)
    {
        writer.Write(value);
    }

    public void Write(bool value)
    {
        writer.Write(value);
    }

    public void Write(int value)
    {
        value = IPAddress.HostToNetworkOrder(value);
        writer.Write(value);
    }

    public void Write(long value)
    {
        value = IPAddress.HostToNetworkOrder(value);
        writer.Write(value);
    }

    public void Write(float value)
    {
        writer.Write(value);
    }

    public void Write(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        Write(bytes);
    }

    public void Write(object obj)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(LitJson.JsonMapper.ToJson(obj));
        Write(bytes);
    }

    public void Write(byte[] bytes) {
        Write((short)bytes.Length);
        writer.Write(bytes);
    }

    public byte[] GetBytes()
    {
        return stream.ToArray();
    }
}
