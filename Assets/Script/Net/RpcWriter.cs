
using System;
using System.IO;
using System.Collections;
using System.Text;

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
        writer.Write(value);
    }

    public void Write(long value)
    {
        writer.Write(value);
    }

    public void Write(float value)
    {
        writer.Write(value);
    }

    public void Write(string value)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        writer.Write(bytes.Length);
    }

    public void Write(object obj)
    {
        //byte[] bytes = Encoding.ASCII.GetBytes(Lot);
        //writer.Write(bytes.Length);
    }

    public byte[] GetBytes()
    {
        return stream.ToArray();
    }
}
