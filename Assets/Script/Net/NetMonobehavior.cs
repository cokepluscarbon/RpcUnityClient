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
            tcpClient.SendBufferSize = 20;
            tcpClient.SendTimeout = 100000;
            tcpClient.Connect("127.0.0.1", 8080);

            writer = new BinaryWriter(tcpClient.GetStream(), System.Text.Encoding.UTF8);
            reader = new BinaryReader(tcpClient.GetStream(), System.Text.Encoding.UTF8);
            Debug.Log("连接服务器成功");

            new Thread(SendWorkThreadFunction).Start();
            // new Thread(ReceiveWorkThreadFunction).Start();
        }
        catch
        {
            Debug.LogError("连接服务器失败，请按回车键退出！");
            Destroy(gameObject);
            return;
        }
    }

    public void SendWorkThreadFunction()
    {
        while (true)
        {
            try
            {
                RpcWriter rpcWriter = new RpcWriter();
                rpcWriter.Write(5);
                rpcWriter.Write("I am Unity Client!");

                RpcRequest.Types.RequestHeader requestHeader = RpcRequest.Types.RequestHeader.CreateBuilder().SetRpcId(5).SetReqId(1).Build();
                RpcRequest rpcRequest = RpcRequest.CreateBuilder().SetHeader(requestHeader).SetContent(ByteString.CopyFrom(rpcWriter.GetBytes())).Build();

                byte[] byteArray = rpcRequest.ToByteArray();
                short mLen = (short)byteArray.Length;

                writer.Write(IPAddress.HostToNetworkOrder(mLen));
                writer.Write(byteArray);

                writer.Flush();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                Debug.LogError(ex.HelpLink);
                Thread.CurrentThread.Abort();
            }

            Thread.Sleep(5000);
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
                Thread.CurrentThread.Abort();
            }
        }
    }
}

