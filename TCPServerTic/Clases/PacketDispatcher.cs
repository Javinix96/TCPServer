using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerTic.Clases
{
    public class PacketDispatcher
    {
        private Dictionary<int, Action<Packet>> packetHandlers = new();

        public void Init()
        {
            packetHandlers = new();
            packetHandlers.Add(1, HandleWelcome);
        }

        private void HandleWelcome(Packet pck)
        {
            string msg = pck.ReadString();
            Console.WriteLine("Mensaje del cliente: " + msg);
        }
    }
}
