using Newtonsoft.Json;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;

namespace TCPServerTic.Clases
{
    public static class PacketFactory
    {

        public static Packet Create<T>(PacketTypeSend header, T dto)
        {

            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteString(JsonConvert.SerializeObject(dto));
            packet.WriteLength();
            return packet;

        }

        public static Packet CreateString(PacketTypeSend header, string dto)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)header);
            packet.WriteString(dto);
            packet.WriteLength();
            return packet;

        }

        public static Packet CreateError(string message)
        {
            Packet packet = new Packet();
            packet.WriteInt((int)PacketTypeSend.Error);
            packet.WriteString(message);
            packet.WriteLength();

            return packet;
        }

    }
}
