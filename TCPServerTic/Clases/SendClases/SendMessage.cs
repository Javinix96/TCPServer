using TCPServerTic.Enums;

namespace TCPServerTic.Clases.SendClases
{
    public class SendMessage : PacketSend
    {
        public override PacketTypeSend Type => PacketTypeSend.Message;

        private string _message;

        public SendMessage(string message) => _message = message;

        public override Packet WritePacket()
        {
            Packet packet = new Packet();
            packet.WriteInt((int)Type);
            packet.WriteString(_message);
            packet.WriteLength();

            return packet;
        }
    }
}
