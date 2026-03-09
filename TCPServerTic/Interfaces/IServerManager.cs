using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases;

namespace TCPServerTic.Interfaces
{
    public  interface IServerManager
    {
        public ClientSession AddClient(TcpClient client, IRoomManager rm);
        public void RemoveClient(ClientSession client);
        public void SendToAll(Packet packet);
    }
}
