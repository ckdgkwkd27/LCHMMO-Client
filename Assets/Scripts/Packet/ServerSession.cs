using System;
using Google.Protobuf;
using UnityEngine;
using ServerCore;
using System.Net;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ServerSession : Session
{
    public ulong playerId;

    public void Send(IMessage packet)
    {
        string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
        PacketID msgId = Util.MessageToID(msgName);
        Debug.Log($"msgId = {msgId}");
        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
        Send(new ArraySegment<byte>(sendBuffer));
    }

    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"OnConnected: {endPoint}");
        Util._networkState = Util.NetworkState.CONNECTED;

        PacketManager.Instance.CustomHandler = (s, m, i) =>
        {
            PacketQueue.Instance.Push(i, m);
        };
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log($"OnDisconnected : {endPoint}");
    }

    public override int OnRecv(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
        return 0;
    }

    public override void OnSend(int numOfBytes)
    {
    }
}
