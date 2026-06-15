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
    public class PlayerRequestJoinRoom : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedJoinRoomRequest;

        private IRoomManager _roomManager;

        public PlayerRequestJoinRoom(RoomManager manager) => _roomManager = manager;

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            var dto = _roomManager.OnPlayerJoin(roomID,client);
            if (dto == null) return;
            var pck2 = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendRequestJoinToRoom, dto);
            client.SendData(pck2);
            var pck = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendPlayersInRoom,dto);
            _roomManager.SendPlayersToARoom(roomID, pck,client.PlayerData.ID);         
        }
    }
}
