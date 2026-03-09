using System.Net;
using TCPServerTic.Clases;
using TCPServerTic.Interfaces;
using TCPServerTic.Routers;

namespace TCPServerTic
{
    class Program
    {
        private static PacketRouter router = null;
        private static IServerManager sm = null;
        private static RoomManager roomManager = null;
        private static TCPServer server = null;

        static async Task Main(string[] args)
        {
            router = new PacketRouter();
            sm = new ServerManager(router);
            roomManager = new RoomManager(sm);
            router.Init(roomManager);
            server = new(IPAddress.Any, 7777,sm,roomManager);
            await server.Start();


        }
    }

}