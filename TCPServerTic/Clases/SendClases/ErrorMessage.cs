using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Enums;

namespace TCPServerTic.Clases.SendClases
{
    public class ErrorMessage : PacketSend
    {
        public override PacketTypeSend Type => PacketTypeSend.Error;

        private string _message;

        public ErrorMessage(string errorMEssage) => _message = errorMEssage;
        

        public override Packet WritePacket()
        {
            Packet packet = new Packet();
            packet.WriteInt((int)PacketTypeSend.Error);
            packet.WriteString(_message);
            packet.WriteLength();

            return packet;
        }
    }
}
