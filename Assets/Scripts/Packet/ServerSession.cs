using System;
using Google.Protobuf;
using UnityEngine;
using ServerCore;
using System.Net;
using Unity.VisualScripting;

public class ServerSession : Session
{
    public void Send(IMessage packet)
    {
        string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
        PacketID msgId = (PacketID)Enum.Parse(typeof(PacketID), msgName);
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
        int processLen = 0;
        return processLen;
    }

    public override void OnSend(int numOfBytes)
    {
        throw new NotImplementedException();
    }
}
