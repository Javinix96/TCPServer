using System;
using TCPServerTic.Clases;
using TCPServerTic.Clases.DTOS;

namespace TCPServerTic.Interfaces
{
    public interface IRoomManager
    {
        Task<PlayerDTO> CreateRoom(ClientSession session,string name);
        RoomInfoDTO GetRooms();
        PlayerDTO GetPlayersInRoom(int roomID);
    }
}
