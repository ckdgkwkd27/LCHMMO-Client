using Google.Protobuf;
using Protocol;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    public static void HandleReturnLogin(ServerSession session, IMessage packet)
    {
        ReturnLogin returnLogin = packet as ReturnLogin;
        if (returnLogin.Success)
        {
            session.playerId = returnLogin.PlayerId;
            Util._networkState = Util.NetworkState.LOGIN;
            Debug.Log($"Login Success!! playerId={session.playerId}");

            SceneManager.LoadScene("StartField");

            RequestEnterGame enterGame = new RequestEnterGame();
            enterGame.PlayerId = session.playerId;
            Managers.Network.Send(enterGame);
        }
    }

    public static void HandleReturnEnterGame(ServerSession session, IMessage packet) 
    {
        ReturnEnterGame returnEnterGame = packet as ReturnEnterGame;

        Debug.Log($"EnterGame Success!, ActorID={returnEnterGame.MyPlayer.ActorId}, ZoneID={returnEnterGame.ZoneId}, PosX={returnEnterGame.MyPlayer.PosInfo.PosX}," +
            $"PosY={returnEnterGame.MyPlayer.PosInfo.PosX}");

        Managers.Map.LoadMap(0);
        Managers.Object.Add(returnEnterGame.MyPlayer, true);
    }

    public static void HandleNotifySpawn(ServerSession session, IMessage packet)
    {
        NotifySpawn spawnPacket = packet as NotifySpawn;
        Debug.Log($"SpawnPacket Size={spawnPacket.Objects.Capacity}");
        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Debug.Log($"OBJ_Name={obj.Name}");
            Managers.Object.Add(obj, false);
        }
    }

    public static void HandleReturnMove(ServerSession session, IMessage packet)
    {
        ReturnMove movePacket = packet as ReturnMove;
        Debug.Log($"MovePacket ActorID={movePacket.ActorId}, PosX={movePacket.PosInfo.PosX}, PosY={movePacket.PosInfo.PosY}");

        GameObject go = Managers.Object.FindById(movePacket.ActorId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer._actorID == movePacket.ActorId)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if(bc == null) 
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void HandleReturnChat(ServerSession session, IMessage packet)
    {

    }
}