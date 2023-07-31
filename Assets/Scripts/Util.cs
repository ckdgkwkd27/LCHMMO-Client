using Google.Protobuf;

public class Util
{
    public static PacketID MessageToID(string msgName)
    {
        switch(msgName)
        {
            case "RequestLogin":
                return PacketID.PKT_CS_LOGIN;
            case "ReturnLogin":
                return PacketID.PKT_SC_LOGIN;
            case "RequestEnterGame":
                return PacketID.PKT_CS_ENTER_GAME;
            case "ReturnEnterGame":
                return PacketID.PKT_SC_ENTER_GAME;

            default:
                return PacketID.PKT_ERROR;
        }
    }
}

