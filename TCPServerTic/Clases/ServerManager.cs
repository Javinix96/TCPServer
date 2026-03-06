using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
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

        public ClientSession AddClient(TcpClient client)
        {
            int id = Interlocked.Increment(ref playersNumber);
            ClientSession sesion = new ClientSession(id, client, this, _packetRouter);

            if (!_players.TryAdd(id, sesion))
                Debug.WriteLine($"Error en guardar al jugador{client.Client.RemoteEndPoint}");

            Console.WriteLine($"jugador agregado {client.Client.RemoteEndPoint} con el id: {id}");
            return sesion;
        }

        public void RemoveClient(ClientSession client)
        {
            if (!_players.TryRemove(client._id, out _))
                Console.WriteLine($"Error en borrar el cliente: {client._id}");
            client._id = Interlocked.Decrement(ref playersNumber);
            Console.WriteLine($"El cliente se ha desconectado: {client._id}");
            //_roomManager.RemovePlayerFromRoom(client._id,client);
        }

        public void SendToAll(Packet packet)
        {
            foreach (ClientSession session in _players.Values)
                session.SendData(packet);
        }
    }
}
