
using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public sealed class NetMakeMove : NetMessage
    {
        public float originalX;
        public float originalZ;
        public float destinationX;
        public float destinationZ;
        public int teamId;
        
        public NetMakeMove()
        {
            Code = OpCode.MAKE_MOVE;
        }

        public NetMakeMove(DataStreamReader reader)
        {
            Code = OpCode.MAKE_MOVE;
            Deserialize(reader);
        }
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte) Code);
            writer.WriteFloat(originalX);
            writer.WriteFloat(originalZ);
            writer.WriteFloat(destinationX);
            writer.WriteFloat(destinationZ);
            writer.WriteInt(teamId);
        }

        public override void Deserialize(DataStreamReader reader)
        {
            originalX = reader.ReadFloat();
            originalZ = reader.ReadFloat();
            destinationX = reader.ReadFloat();
            destinationZ = reader.ReadFloat();
            teamId = reader.ReadInt();
        }
        public override void ReceivedOnClient()
        {
            NetUtility.C_MAKE_MOVE?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            NetUtility.S_MAKE_MOVE?.Invoke(this,cnn);
        }
    }
}
