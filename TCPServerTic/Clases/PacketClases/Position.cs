using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketClases
{
    internal class Position : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedPosition;

        private RoomManager _roomManager;

        public Position(RoomManager  rm) => _roomManager = rm;

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            string player = payload.ReadString();
            int index = payload.ReadInt();
            
            _roomManager.RoomPosition(roomID, player, index);
        }
    }
}
