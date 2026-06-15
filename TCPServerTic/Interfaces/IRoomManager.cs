using TCPServerTic.Clases;
using TCPServerTic.Clases.DTOS;

namespace TCPServerTic.Interfaces
{
    public interface IRoomManager
    {
        Task<PlayerDTO> CreateRoom(ClientSession session,string name, string password, bool isPrivate, int time);
        RoomInfoDTO GetRooms();
        PlayerDTO GetPlayersInRoom(int roomID);
        PlayerDTO OnPlayerJoin(int roomId, ClientSession client,bool userWrotePass = false, string password = "");
        void SendPlayersToARoom(int roomID, Packet pck, int idSended = 0);
        PlayerDTO RemovePlayerFromRoom(int roomID, ClientSession session, int times = 1);
        bool CanLoadSceneGame(int clientID,int id);
    }
}
