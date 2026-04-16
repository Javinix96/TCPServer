using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.SendClases
{
    public class SendRooms : IPacketHandler
    {
        public int Header => (int)PacketTypeSend.SendRoomList;

        private RoomManager _roomManager;

        public SendRooms(RoomManager rm) => _roomManager = rm;
        

        public void Handle(ClientSession client, Packet payload)
        {
            var dto = _roomManager.GetRooms();
            var packet = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.SendRoomList, dto);
          
            client.SendData(packet);
        }
    }
}
