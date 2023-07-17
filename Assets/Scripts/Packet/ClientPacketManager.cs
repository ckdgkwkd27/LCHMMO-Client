using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

class PacketManager
{
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }

    PacketManager()
    {
        Register();
    }

    public void Register()
    {

    }



    public Action<ServerSession, IMessage> GetPacketHandler(ushort id)
    {
        return null;
    }
}