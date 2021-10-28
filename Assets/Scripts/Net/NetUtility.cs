using System;
using Net.NetMessage;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net
{
    public enum OpCode
    {
        KEEP_ALIVE = 1,
        WELCOME = 2,
        START_GAME = 3,
        MAKE_MOVE = 4,
        REMATCH = 5
    }

    public static class NetUtility
    {
        public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
        {
            NetMessage.NetMessage msg = null;
            var opCode = (OpCode) stream.ReadByte();
            switch (opCode)
            {
                case OpCode.KEEP_ALIVE:
                    msg = new NetKeepAlive(stream);
                    break;
                case OpCode.WELCOME:
                    msg = new NetWelcome(stream);
                    break;
                case OpCode.START_GAME:
                    msg = new NetStartGame(stream);
                    break;
                case OpCode.MAKE_MOVE:
                    msg = new NetMakeMove(stream);
                    break;
                case OpCode.REMATCH:
                    msg = new NetRematch(stream);
                    break;
                default:
                    Debug.LogError("Mesage received had no OpCode");
                    break;
            }

            if (server != null)
            {
                msg?.ReceivedOnServer(cnn);
                return;
            }
            msg?.ReceivedOnClient();
        }

        public static Action<NetMessage.NetMessage> C_KEEP_ALIVE;
        public static Action<NetMessage.NetMessage> C_WELCOME;
        public static Action<NetMessage.NetMessage> C_START_GAME;
        public static Action<NetMessage.NetMessage> C_MAKE_MOVE;
        public static Action<NetMessage.NetMessage> C_REMATCH;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_KEEP_ALIVE;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_WELCOME;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_START_GAME;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_MAKE_MOVE;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_REMATCH;
    }
}