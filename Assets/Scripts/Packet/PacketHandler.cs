using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

class PacketHandler
{
    public static void HandleReturnLogin(ServerSession session, IMessage packet)
    {
        ReturnLogin returnLogin = packet as ReturnLogin;
        if (returnLogin.Success)
        {
            session.playerId = returnLogin.PlayerId;

            //#TODO
            //Player 버튼 UI를 띄운다
        }
    }

    public static void HandleReturnEnterGame(ServerSession session, IMessage packet) 
    { 
        ReturnEnterGame returnEnterGame = packet as ReturnEnterGame;

        //내가 스폰되었다
        if (session.playerId == returnEnterGame.PlayerId)
        {
            /* <Plan>
             * UI 만들어서 그걸로 RequestLogin Packet 보내기
             * Actor Pool 만들어서 Object Pooling 하기
             * Player Spawn 구현 이후 Moving
            */
        }

        //남이 스폰되었다
        else
        {

        }
    }

    public static void HandleReturnChat(ServerSession session, IMessage packet)
    {

    }
}