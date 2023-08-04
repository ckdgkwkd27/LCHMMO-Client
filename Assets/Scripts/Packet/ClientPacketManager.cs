using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Unity.VisualScripting;
using UnityEngine.XR;

public enum PacketID
{
    PKT_CS_LOGIN = 1,
    PKT_SC_LOGIN,
    PKT_CS_ENTER_GAME,
    PKT_SC_ENTER_GAME,
    PKT_CS_CHAT,
    PKT_SC_CHAT,

    PKT_ERROR,
};

class PacketManager
{
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<ServerSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<ServerSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<ServerSession, IMessage>> _handler = new Dictionary<ushort, Action<ServerSession, IMessage>>();
    public Action<ServerSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)PacketID.PKT_SC_LOGIN, MakePacket<Protocol.ReturnLogin>);
        _handler.Add((ushort)PacketID.PKT_SC_LOGIN, PacketHandler.HandleReturnLogin);
        _onRecv.Add((ushort)PacketID.PKT_SC_ENTER_GAME, MakePacket<Protocol.ReturnEnterGame>);
        _handler.Add((ushort)(PacketID.PKT_SC_ENTER_GAME), PacketHandler.HandleReturnEnterGame);
        _onRecv.Add((ushort)PacketID.PKT_SC_CHAT, MakePacket<Protocol.ReturnChat>);
        _handler.Add((ushort)(PacketID.PKT_SC_CHAT), PacketHandler.HandleReturnChat);

    }

    public void OnRecvPacket(ServerSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<ServerSession, ArraySegment<byte>, ushort> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);
    }

    void MakePacket<T>(ServerSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        if(CustomHandler != null)
        {
            CustomHandler.Invoke(session, pkt, id);
        }

        else
        {
            Action<ServerSession, IMessage> action = null;
            if(_handler.TryGetValue(id, out action))
            {
                action.Invoke(session, pkt);
            }
        }
    }

    public Action<ServerSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<ServerSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            return action;
        return null;
    }
}