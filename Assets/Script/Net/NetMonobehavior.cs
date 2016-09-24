using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using com.ckcb.gs;
using Google.ProtocolBuffers;
using System.IO;

public class NetMonoBehavior : MonoBehaviour
{
    private TcpClient tcpClient;
    private BinaryWriter writer;
    private BinaryReader reader;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));

            writer = new BinaryWriter(tcpClient.GetStream());
            reader = new BinaryReader(tcpClient.GetStream());
            Debug.Log("连接服务器成功");
        }
        catch
        {
            Debug.LogError("连接服务器失败，请按回车键退出！");
            Destroy(gameObject);
            return;
        }

        new Thread(SendWorkThreadFunction).Start();
        new Thread(ReceiveWorkThreadFunction).Start();
    }

    public void SendWorkThreadFunction()
    {
        while (true)
        {
            try
            {
                string content = "I am Unity Client!";
                RpcRequest.Types.RequestHeader requestHeader = RpcRequest.Types.RequestHeader.CreateBuilder().SetRpcId(5).SetReqId(1).Build();
                RpcRequest rpcRequest = RpcRequest.CreateBuilder().SetHeader(requestHeader).SetContent(ByteString.CopyFromUtf8(content)).Build();


                byte[] byteArray = rpcRequest.ToByteArray();
                writer.Write(byteArray.Length);
                writer.Write(byteArray);
                writer.Flush();

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

    public void ReceiveWorkThreadFunction()
    {
        while (true)
        {
            try
            {
                int len = reader.ReadInt32();
                byte[] byteArray = reader.ReadBytes(len);

                RpcResponse rpcResponse = RpcResponse.ParseFrom(ByteString.CopyFrom(byteArray));

                System.Console.WriteLine(rpcResponse.Content.ToStringUtf8());

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}

