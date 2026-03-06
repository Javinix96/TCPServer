using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases;
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

        public void Init(RoomManager rm)
        {
            _handlers = new Dictionary<int, IPacketHandler>()
            {
                { (int)PacketTypeReceive.Welcome, new Welcome() },
                { (int)PacketTypeReceive.RequestRooms, new RequestedRooms(rm) },
                { (int)PacketTypeReceive.createRoom, new RoomCreate(rm) },
                { (int)PacketTypeSend.RoomList, new SendRooms(rm) },
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
