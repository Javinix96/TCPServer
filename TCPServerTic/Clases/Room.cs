
using System.Collections.Concurrent;
using TCPServerTic.Clases.SendClases;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases
{
    public class Room : IRoom
    {
        private ConcurrentDictionary<int,ClientSession> _roomPlayers = new();
        private int _roomID;
        private string _roomName;


        public string RoomName => _roomName;
        public int RoomID => _roomID;

        public Room(int roomID,string roomName) {
            _roomPlayers = new ConcurrentDictionary<int, ClientSession>(2,2);
            _roomID = roomID;
            _roomName = roomName;
        }

        public bool AddPlayer(ClientSession player)
        {
            if (!TryAddPlayer(player._id, player))
                return false;

            return true;
        }

        public void RemovePlayer(ClientSession player)
        {
            if (player == null)
                return;

            _roomPlayers.TryRemove(player._id,out _);
        }


        public async Task SendDataToPlayers()
        {
           await Task.Delay(500);
        }

        private bool TryAddPlayer(int id, ClientSession session)
        {
            if (_roomPlayers.TryAdd(id, session))
            {
                if (_roomPlayers.Count > 2)
                {
                    _roomPlayers.TryRemove(id, out _);
                    return false;
                }
                return true;
            }

            return false;
        }

        public ClientSession[] GetPlayers()
        {
           return _roomPlayers.Values.ToArray();
        }
    }
}
