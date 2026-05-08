using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class RequestedRooms : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedRequestRooms;
        private RoomManager _roomManager;

        public RequestedRooms(RoomManager roomManager) => _roomManager = roomManager;
        
        public void Handle(ClientSession client, Packet payload)
        {
            var dto = _roomManager.GetRooms();
            Packet pck = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.SendRoomList, dto);
            client.SendData(pck);
        }
    }
}
