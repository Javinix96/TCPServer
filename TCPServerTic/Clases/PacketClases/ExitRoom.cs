using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;
using TCPServerTic.Routers;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class ExitRoom : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedExitRoom;

        private IRoomManager _roomManager;
        private IServerManager _serverManager;

        public ExitRoom(RoomManager rm, ServerManager sm)
        {
            _roomManager = rm;
            _serverManager = sm;
        }

        public void Handle(ClientSession client, Packet payload)
        {
            int roomID = payload.ReadInt();
            var dto = _roomManager.RemovePlayerFromRoom(roomID, client);
            var pck = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendPlayersInRoom, dto);
            _roomManager.SendPlayersToARoom(roomID, pck);

            var roomsDTO = _roomManager.GetRooms();
            var pck2 = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.SendRoomList, roomsDTO);
            _serverManager.SendToAll(pck2);
        }
    }
}
