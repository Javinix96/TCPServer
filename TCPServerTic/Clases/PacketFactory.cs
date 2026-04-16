using Newtonsoft.Json;
using TCPServerTic.Enums;

namespace TCPServerTic.Clases
{
    public static class PacketFactory
    {
        public static Packet Create<T>(PacketTypeSend header, T dto)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteBool(true);
            packet.WriteString(JsonConvert.SerializeObject(dto));
            packet.WriteLength();
            return packet;
        }

        public static Packet CreateString(PacketTypeSend header, string mess)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteBool(false);//no es json solo string
            packet.WriteString(mess);
            packet.WriteLength();
            return packet;
        }

        public static Packet SendBool(PacketTypeSend header, bool bb)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteBool(bb);
            packet.WriteLength();
            return packet;
        }

        public static Packet SendInt(PacketTypeSend header, int number)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteInt(number);
            packet.WriteLength();
            return packet;
        }
        public static Packet SendPos(PacketTypeSend header, string player,int index)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteString(player);
            packet.WriteInt(index);
            packet.WriteLength();
            return packet;
        }


        public static Packet CreateError(string message)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)PacketTypeSend.SendError);
            packet.WriteString(message);
            packet.WriteLength();
            return packet;
        }
    }
}
