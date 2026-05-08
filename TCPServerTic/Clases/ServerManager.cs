using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;
using TCPServerTic.Routers;

namespace TCPServerTic.Clases
{
    public class ServerManager : IServerManager
    {
        private ConcurrentDictionary<int, ClientSession> _players = null;
        private PacketRouter _packetRouter;
        int playersNumber = 0;

        public ServerManager(PacketRouter pr)
        {
            _players = new();
            _packetRouter = pr;

        }

        public ClientSession AddClient(TcpClient client, IRoomManager rm)
        {
            int id = Interlocked.Increment(ref playersNumber);
            ClientSession sesion = new ClientSession(id, client, this, _packetRouter,rm);

            if (!_players.TryAdd(id, sesion))
                Debug.WriteLine($"Error en guardar al jugador{client.Client.RemoteEndPoint}");

            Console.WriteLine($"jugador agregado {client.Client.RemoteEndPoint} con el id: {id}");

            Packet pck = new Packet();
            pck.WriteInt((int)PacketTypeSend.SendAccept);
            pck.WriteInt(id);
            pck.WriteString("Bienvenido al server");
            pck.WriteLength();
            sesion.SendData(pck);

            //sesion.SendData(PacketFactory.CreateString(PacketTypeSend.SendAccept,"Bienvenido al server"));

            return sesion;
        }

        public void RemoveClient(ClientSession client)
        {
            if (!_players.TryRemove(client.PlayerData.ID, out _))
                Console.WriteLine($"Error en borrar el cliente: {client.PlayerData.ID}");
            //int clintID = client.PlayerData.ID;
            //client.PlayerData.ID = Interlocked.Decrement(ref playersNumber);
            Console.WriteLine($"El cliente se ha desconectado: {client.PlayerData.ID}");
            //clintID = 0;
        }

        public void SendToAll(Packet packet)
        {
            foreach (ClientSession session in _players.Values)
                session.SendData(packet);
        }

        public void Disconnect(ClientSession client)
        {
            RemoveClient(client);
            client.Close();
        }
    }
}
