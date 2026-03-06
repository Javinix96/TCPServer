using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class RoomCreate : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.createRoom;
        private RoomManager _roomManager;

        public RoomCreate(RoomManager roomManager)
        {
            _roomManager = roomManager;
        }

        public async void Handle(ClientSession client, Packet payload)
        {
            string nameRoom = payload.ReadString();
            var room = await _roomManager.CreateRoom(client,nameRoom);

           var packet = PacketFactory.Create<PlayerDTO>(PacketTypeSend.Message, room);
           client.SendData(packet);
        }
    }
}
