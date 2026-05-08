using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases;
using TCPServerTic.Clases.PacketClases;
using TCPServerTic.Clases.PacketRecieve;
using TCPServerTic.Clases.PAcketRecieve;
using TCPServerTic.Clases.SendClases;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Routers
{
    public class PacketRouter
    {
        private Dictionary<int, IPacketHandler> _handlers;

        public void Init(RoomManager rm, ServerManager sm)
        {
            _handlers = new Dictionary<int, IPacketHandler>()
            {
                { (int)PacketTypeReceive.ReceivedWelcome, new Welcome() },
                { (int)PacketTypeReceive.ReceivedRequestRooms, new RequestedRooms(rm) },
                { (int)PacketTypeReceive.ReceivedcreateRoom, new RoomCreate(rm) },
                { (int)PacketTypeReceive.ReceivedExitRoom, new ExitRoom(rm,sm) },
                { (int)PacketTypeReceive.ReceivedJoinRoomRequest, new PlayerRequestJoinRoom(rm) },
                { (int)PacketTypeReceive.ReceivedPlayerReady, new PlayerReady(rm) },
                { (int)PacketTypeReceive.ReceivedRequestJoin, new LoadGame(rm) },
                { (int)PacketTypeReceive.ReceivedPosition, new Position(rm) },
                { (int)PacketTypeReceive.ReceivedReadyPos, new Board(rm) },
                { (int)PacketTypeReceive.ReceivedExit, new ExitPlayer(sm) },
                { (int)PacketTypeReceive.ReceivedName, new WriteName() },
                { (int)PacketTypeReceive.ReceivedSearchRoom, new SearchRoom(rm) },
                { (int)PacketTypeReceive.ReceivedPassword, new RoomPassword(rm) },
                { (int)PacketTypeSend.SendRoomList, new SendRooms(rm) },
            };
        }

        public void Route(byte header, Packet payload, ClientSession client)
        {
            if (_handlers.TryGetValue(header, out var handler))
                handler.Handle(client,payload);
            else
                Console.WriteLine($"No handler for header {header}");
        }
    }
}
