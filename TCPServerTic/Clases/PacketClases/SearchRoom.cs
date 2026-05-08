using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketClases
{
    public class SearchRoom : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedSearchRoom;
        private RoomManager _roomManager;

        public SearchRoom(RoomManager roomManager) => _roomManager = roomManager;
        

        public void Handle(ClientSession client, Packet payload)
        {
            string roomName = payload.ReadString();
            _roomManager.SearchRoom(client, roomName);
        }
    }
}
