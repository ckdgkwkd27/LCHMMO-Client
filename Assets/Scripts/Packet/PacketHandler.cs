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
            Util.playerId = session.playerId;
            Debug.Log($"Login Success!! playerId={session.playerId}");
        }
    }

    public static void HandleReturnEnterGame(ServerSession session, IMessage packet) 
    {
        ReturnEnterGame returnEnterGame = packet as ReturnEnterGame;

        //내가 스폰되었다
        if (Util.actorId == returnEnterGame.ActorId)
        {
            Debug.Log($"EnterGame Success!, ActorID={returnEnterGame.ActorId}, ZoneID={returnEnterGame.ZoneId}, PosX={returnEnterGame.PosX}," +
                $"PosY={returnEnterGame.PosY}");

            Managers.Clear();
            SceneManager.LoadScene("SampleScene");
        }

        //남이 스폰되었다
        else
        {
            Debug.Log($"ME={Util.actorId} and packet.playerId={returnEnterGame.ActorId}");
        }
    }

    public static void HandleReturnChat(ServerSession session, IMessage packet)
    {

    }
}