using System.Net;
using TCPServerTic.Clases;
using TCPServerTic.Routers;

namespace TCPServerTic
{
    class Program
    {
        private static PacketRouter router = null;
        private static ServerManager sm = null;
        private static RoomManager roomManager = null;
        private static TCPServer server = null;

        static async Task Main(string[] args)
        {
            router = new PacketRouter();
            sm = new ServerManager(router);
            roomManager = new RoomManager(sm);
            router.Init(roomManager,sm);
            server = new(IPAddress.Any, 7777,sm,roomManager);
            await server.Start();
        }
    }

}