using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases;

namespace TCPServerTic.Interfaces
{
    public interface IPacketHandler
    {
        int Header { get; }
        void Handle(ClientSession client, Packet payload);
     }
}
