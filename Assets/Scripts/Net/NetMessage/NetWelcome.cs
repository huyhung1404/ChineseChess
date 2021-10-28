using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public sealed class NetWelcome : NetMessage
    {
        public int AssignedTeam { get; set; }
        public NetWelcome()
        {
            Code = OpCode.WELCOME;
        }

        public NetWelcome(DataStreamReader reader)
        {
            Code = OpCode.WELCOME;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte) Code);
            writer.WriteInt(AssignedTeam);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            AssignedTeam = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.C_WELCOME?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.S_WELCOME?.Invoke(this,cnn);
        }
    }
}
