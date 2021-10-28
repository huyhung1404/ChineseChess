using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public class NetKeepAlive : NetMessage
    {
        public NetKeepAlive() //Tạo ra dữ liệu
        {
            Code = OpCode.KEEP_ALIVE;
        }

        public NetKeepAlive(DataStreamReader reader) //Nhận dữ liệu
        {
            Code = OpCode.KEEP_ALIVE;
            Deserialize(reader);
        }

        //Đóng gói dữ liệu
        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte) Code);
        }
        //Giải nén dữ liệu nhận được
        public override void Deserialize(DataStreamReader reader)
        {
        
        }
        public override void ReceivedOnClient()
        {
            //Thực thi sự kiên keep_alive trong client
            NetUtility.C_KEEP_ALIVE?.Invoke(this);
        }
        public override void ReceivedOnServer(NetworkConnection cnn)
        {
            //Thực thi sự kiện trong server
            NetUtility.S_KEEP_ALIVE?.Invoke(this,cnn);
        }
    }
}
