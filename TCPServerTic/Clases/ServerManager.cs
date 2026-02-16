using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;

namespace TCPServerTic.Clases
{
    public class ServerManager
    {
        private static readonly Lazy<ServerManager> _instance =
        new Lazy<ServerManager>(() => new ServerManager());
        private ConcurrentDictionary<int, ClientSession> _players = null;
        int playersNumber = 0;

        public static ServerManager SM => _instance.Value;

        public void Init()
        {
            _players = new();
        }

        public ClientSession AddClient(TcpClient client)
        {
            int id = Interlocked.Increment(ref playersNumber);
            ClientSession sesion = new ClientSession(id, client);

            if (!_players.TryAdd(id, sesion))
                Debug.WriteLine($"Error en guardar al jugador{client.Client.RemoteEndPoint}");

            Console.WriteLine($"jugador agregado {client.Client.RemoteEndPoint} con el id: {id}");
            return sesion;
        }

        public void RemoveClient(ClientSession client)
        {
            if (!_players.TryRemove(client._id, out _))
                Console.WriteLine($"Error en borrar el cliente: {client._id}");
            Interlocked.Decrement(ref playersNumber);
            Console.WriteLine($"El cliente se ha desconectado: {client._id}");

        }


    }
}
