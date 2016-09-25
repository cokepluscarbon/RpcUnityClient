using System;
using com.ckcb.gs;
using System.Collections.Generic;
using Google.ProtocolBuffers;

public class Net
{
    private static Net _instance;
    public static Net Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Net();
            }
            return _instance;
        }
    }
    public Queue<RpcRequest> sendQueue = new Queue<RpcRequest>();
    public Queue<RpcResponse> receiveQueue = new Queue<RpcResponse>();
    private int currReqId;

    public static void Rpc(string rpc, object arg)
    {
        Rpc(rpc, new object[] { arg }, null);
    }

    public static void Rpc(string rpc, object arg, Action<RpcReader> callback)
    {
        Rpc(rpc, new object[] { arg }, callback);
    }

    public static void Rpc(string rpc, object[] args, Action<RpcReader> callback)
    {
        RpcWriter writer = new RpcWriter();
        for (int i = 0; i < args.Length; i++)
        {
            writer.Write((int)args[i]);
        }

        RpcRequest.Types.RequestHeader header = RpcRequest.Types.RequestHeader.CreateBuilder().SetRpcId(5).SetReqId(Instance.currReqId++).Build();
        RpcRequest request = RpcRequest.CreateBuilder().SetHeader(header).SetContent(ByteString.CopyFrom(writer.GetBytes())).Build();

        Instance.sendQueue.Enqueue(request);
    }

    public static void T(params object[] agrs)
    {
        Net.Rpc("rpc_get_player", new object[] { 10086 }, reader =>
        {
            int rpcId = reader.ReadInt();
        });
    }
}
