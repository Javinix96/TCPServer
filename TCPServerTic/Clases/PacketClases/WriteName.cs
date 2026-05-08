using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketClases
{
    public class WriteName : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedName;

        public void Handle(ClientSession client, Packet payload)
        {
            string name = payload.ReadString();
            Console.WriteLine(name);
            client.PlayerData.Name = name;
        }
    }
}
