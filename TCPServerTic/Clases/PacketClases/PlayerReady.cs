using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class PlayerReady : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedPlayerReady;

        private RoomManager _roomManager;

        public PlayerReady(RoomManager rm) => _roomManager = rm;

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            int playerID = payload.ReadInt();

            if (playerID != client.PlayerData.ID)
            {
                Console.WriteLine("Los id no Coinciden");
                return;
            }

            _roomManager.SetReadyPlayer(roomID,playerID,client);
        }
    }
}
