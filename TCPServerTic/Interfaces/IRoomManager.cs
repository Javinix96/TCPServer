using System;
using System.Xml.Linq;
using TCPServerTic.Clases;
using TCPServerTic.Clases.DTOS;

namespace TCPServerTic.Interfaces
{
    public interface IRoomManager
    {
        Task<PlayerDTO> CreateRoom(ClientSession session,string name, string password, bool isPrivate, int time);
        RoomInfoDTO GetRooms();
        PlayerDTO GetPlayersInRoom(int roomID);
        PlayerDTO OnPlayerJoin(int roomId, ClientSession client);
        void SendPlayersToARoom(int roomID, Packet pck);
        PlayerDTO RemovePlayerFromRoom(int roomID, ClientSession session);
        bool CanLoadSceneGame(int clientID,int id);
    }
}
