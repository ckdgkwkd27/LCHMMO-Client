using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log($"LoginSuccess!! playerId={session.playerId}");
        }
    }

    public static void HandleReturnEnterGame(ServerSession session, IMessage packet) 
    {
        Debug.Log("HandleReturnEnterGame()");
        ReturnEnterGame returnEnterGame = packet as ReturnEnterGame;

        //내가 스폰되었다
        if (Util.playerId == returnEnterGame.PlayerId)
        {
            Debug.Log("Return Enter Game!");
        }

        //남이 스폰되었다
        else
        {
            Debug.Log($"ME={Util.playerId} and packet.playerId={returnEnterGame.PlayerId}");
        }
    }

    public static void HandleReturnChat(ServerSession session, IMessage packet)
    {

    }
}