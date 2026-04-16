using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketClases
{
    public class Board : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedReadyPos;

        private RoomManager _roomManager;

        public Board(RoomManager rm) => _roomManager = rm;


        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            string player = payload.ReadString();
            int index = payload.ReadInt();

            _roomManager.UpdateBoard(roomID, player, index);
        }
    }
}
