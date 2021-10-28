using System;
using Managers;
using Net.NetMessage;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net
{
    public class Server : Singleton<Server>
    {
        private NetworkDriver driver;
        private NativeList<NetworkConnection> m_Connections;
        private bool _isActive;
        private const float _keepAliveTickRate = 20.0f;
        private float _lastKeepAlive;

        // public Action connectionDropped;

        public void Init(ushort port)
        {
            driver = NetworkDriver.Create();
            NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
            endPoint.Port = port;
            if (driver.Bind(endPoint) != 0)
            {
                return;
            }
            driver.Listen();
            RegisterToEvent();
            m_Connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
            _isActive = true;
        }

        public void Shutdown()
        {
            if (!_isActive) return;
            UnregisterToEvent();
            driver.Dispose();
            m_Connections.Dispose();
            _isActive = false;
        }

        public void OnDestroy()
        {
            Shutdown();
        }

        private void Update()
        {
            if (!_isActive) return;

            KeepAlive();
            driver.ScheduleUpdate().Complete();
            CleanupConnections();
            AcceptNewConnections();
            UpdateMessagePump();
        }

        private void KeepAlive()
        {
            if (!(Time.time - _lastKeepAlive > _keepAliveTickRate)) return;
            _lastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }

        private void CleanupConnections()
        {
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (m_Connections[i].IsCreated) continue;
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        private void AcceptNewConnections()
        {
            NetworkConnection c;
            while ((c = driver.Accept()) != default)
            {
                m_Connections.Add(c);
            }
        }

        private void UpdateMessagePump()
        {
            for (int i = 0; i < m_Connections.Length; i++)
            {
                NetworkEvent.Type cmd;
                while ((cmd = driver.PopEventForConnection(m_Connections[i], out DataStreamReader stream)) !=
                       NetworkEvent.Type.Empty)
                {
                    switch (cmd)
                    {
                        case NetworkEvent.Type.Data:
                            NetUtility.OnData(stream, m_Connections[i], this);
                            break;
                        case NetworkEvent.Type.Disconnect:
                            m_Connections[i] = default;
                            // connectionDropped?.Invoke();
                            Shutdown();
                            break;
                        case NetworkEvent.Type.Empty:
                            break;
                        case NetworkEvent.Type.Connect:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public void SendToClient(NetworkConnection connection, NetMessage.NetMessage msg)
        {
            driver.BeginSend(connection, out DataStreamWriter writer);
            msg.Serialize(ref writer);
            driver.EndSend(writer);
        }

        public void Broadcast(NetMessage.NetMessage msg)
        {

            foreach (NetworkConnection connection in m_Connections)
            {
                if (connection.IsCreated)
                {
                    SendToClient(connection, msg);
                }
            }
        }
        private void RegisterToEvent()
        {
            NetUtility.S_KEEP_ALIVE += OnKeepAliveServer;
        }

        private void UnregisterToEvent()
        {
            NetUtility.S_KEEP_ALIVE -= OnKeepAliveServer;
        }

        private void OnKeepAliveServer(NetMessage.NetMessage nm, NetworkConnection cnn)
        {
            Debug.Log("Connecting");
        }
        
    }
}