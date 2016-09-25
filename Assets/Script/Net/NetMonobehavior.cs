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
    private Dictionary<int, Action<RpcReader>> callbacks = new Dictionary<int, Action<RpcReader>>();
    private TcpClient tcpClient;
    private BinaryWriter writer;
    private BinaryReader reader;

    void Awake()
    {
        TableLoader.Load<int>("");

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
            Net.Rpc("rpc_get_player", 1, reader =>
            {
                Debug.Log("================");
            });

            Debug.Log("Send Count = " + Net.Instance.sendQueue.Count);
            if (Net.Instance.sendQueue.Count > 0)
            {
                RpcRequest request = Net.Instance.sendQueue.Dequeue();

                byte[] byteArray = request.ToByteArray();

                writer.Write(IPAddress.HostToNetworkOrder(byteArray.Length));
                writer.Write(byteArray);

                writer.Flush();
            }
            Thread.Sleep(1000);
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

