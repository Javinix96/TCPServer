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
    public class PlayerJoinRoom : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.JoinRoomRequest;

        private IRoomManager _roomManager;

        public PlayerJoinRoom(RoomManager manager) => _roomManager = manager;

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            var dto = _roomManager.OnPlayerJoin(roomID,client);
            var pck = PacketFactory.Create<PlayerDTO>(PacketTypeSend.JoinRoom,dto);
            _roomManager.SendPlayerInARoom(roomID, pck);         
        }
    }
}
