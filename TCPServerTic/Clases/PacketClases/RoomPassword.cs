using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketClases
{
    public class RoomPassword : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedPassword;
        private RoomManager _roomManager;

        public RoomPassword(RoomManager roomManager) => _roomManager = roomManager;

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            string password = payload.ReadString();
            var dto =_roomManager.OnPlayerJoin(roomID,client,true,password);
            var pck2 = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendRequestJoinToRoom, dto);
            client.SendData(pck2);
            var pck = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendPlayersInRoom, dto);
            _roomManager.SendPlayersToARoom(roomID, pck);

        }

    }
}
