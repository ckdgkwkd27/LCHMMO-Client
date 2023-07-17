using System;
using Google.Protobuf;
using UnityEngine;
using ServerCore;
using System.Net;

public class ServerSession : Session
{
    public void Send(IMessage packet)
    {

    }

    public override void OnConnected(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public override int OnRecv(ArraySegment<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public override void OnSend(int numOfBytes)
    {
        throw new NotImplementedException();
    }
}
