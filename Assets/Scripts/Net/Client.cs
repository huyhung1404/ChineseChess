using System;
using Managers;
using Net.NetMessage;
using Unity.Networking.Transport;

namespace Net
{
    public class Client : Singleton<Client>
    {
        private NetworkDriver driver;
        private NetworkConnection _connection;
        private bool _isActive;

        // public Action connectionDropped;

        public void Init(string ip, ushort port)
        {
            driver = NetworkDriver.Create();
            NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port);
            _connection = driver.Connect(endPoint);
            _isActive = true;
            RegisterToEvent(); 
        }

        public void Shutdown()
        {
            if (!_isActive) return;
            UnregisterToEvent();
            driver.Dispose();
            _isActive = false;
            _connection = default;
        }

        public void OnDestroy()
        {
            Shutdown();
        }

        private void Update()
        {
            if (!_isActive) return;

            driver.ScheduleUpdate().Complete();

            CheckAlive();
            UpdateMessagePump();
        }

        private void CheckAlive()
        {
            if (!_connection.IsCreated && _isActive)
            {
                // connectionDropped?.Invoke();
                Shutdown();
            }
        }


        private void UpdateMessagePump()
        {
            NetworkEvent.Type cmd;
            while ((cmd = _connection.PopEvent(driver, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Connect:
                        SendToServer(new NetWelcome());
                        break;
                    case NetworkEvent.Type.Data:
                        NetUtility.OnData(stream,default);
                        break;
                    case NetworkEvent.Type.Disconnect:
                        _connection = default;
                        // connectionDropped?.Invoke();
                        Shutdown();
                        break;
                    case NetworkEvent.Type.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SendToServer(NetMessage.NetMessage msg)
        {
            driver.BeginSend(_connection, out DataStreamWriter writer);
            msg.Serialize(ref writer);
            driver.EndSend(writer);
        }

        private void RegisterToEvent()
        {
            NetUtility.C_KEEP_ALIVE += OnKeepAlive;
        }

        private void UnregisterToEvent()
        {
            NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
        }

        private void OnKeepAlive(NetMessage.NetMessage nm)
        {
            SendToServer(nm);
        }
    }
}