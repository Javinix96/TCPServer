using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Enums;

namespace TCPServerTic.Clases
{
    public abstract class PacketSend
    {
        public abstract PacketTypeSend Type { get; }
        public abstract Packet WritePacket();
    }
}
