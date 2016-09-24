using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class NetMonoBehaviour : MonoBehaviour
{
    private Socket socket;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
            Debug.Log("连接服务器成功");
        }
        catch
        {
            Console.WriteLine("连接服务器失败，请按回车键退出！");
            return;
        }

        new Thread(SendWorkThreadFunction).Start();
        new Thread(ReceiveWorkThreadFunction).Start();
    }

    public void SendWorkThreadFunction()
    {
        try
        {
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
    }

    public void ReceiveWorkThreadFunction()
    {
        try
        {
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }
    }
}

