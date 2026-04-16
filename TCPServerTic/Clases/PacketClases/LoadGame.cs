using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class LoadGame : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedRequestJoin;

        private IRoomManager _roomManager;
        public LoadGame(RoomManager rm) => _roomManager = rm;

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();

            bool canLoad = _roomManager.CanLoadSceneGame(client.PlayerData.ID, roomID);

            Packet pck = PacketFactory.SendBool(PacketTypeSend.SendLoadScene, canLoad);
            
            client.SendData(pck);
        }
    }
}
