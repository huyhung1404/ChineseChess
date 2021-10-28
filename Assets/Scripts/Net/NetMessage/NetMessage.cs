using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public class NetMessage
    {
        protected OpCode Code { get; set; }

        public virtual void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte) Code);
        }

        public virtual void Deserialize(DataStreamReader reader)
        {
        
        }
        public virtual void ReceivedOnClient()
        {
        
        }
        public virtual void ReceivedOnServer(NetworkConnection cnn)
        {
        
        }
    }
}
