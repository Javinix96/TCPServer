using System.Collections.Concurrent;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases
{
    public class RoomManager : IRoomManager
    {
        private IServerManager _serverManager;
        private ConcurrentDictionary<int, Room> _rooms = null;

        int roomId = 0;

        public RoomManager(IServerManager sm) { 
            _rooms = new ConcurrentDictionary<int, Room>();
            _serverManager = sm;
        }

        public async Task<PlayerDTO> CreateRoom(ClientSession session,string name)
        {
            int newRoomId = Interlocked.Increment(ref roomId);

            PlayerDTO dto = new PlayerDTO();

            Room newRoom = new Room(newRoomId,name);

            if (!_rooms.TryAdd(newRoomId, newRoom))
            {
                Console.WriteLine($"Failed to create room with ID {newRoomId}");
                dto.Success = false;
                dto.Message = "Error on createrd a room";
                return null;
            }

            newRoom.AddPlayer(session);

            await Task.Delay(200);

            var roomsdto = GetRooms();
            
            var ss = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.RoomList, roomsdto);
            
            _serverManager.SendToAll(ss);

            await Task.Delay(200);

            dto = GetPlayersInRoom(newRoomId);

            return dto;
        }

        public RoomInfoDTO GetRooms()
        {
            RoomInfoDTO dto = new RoomInfoDTO();
            dto.Count = _rooms.Count;
            var rooms = _rooms.Values.Select(r => new RoomInfo
            {
                RoomId = r.RoomID,
                RoomName = r.RoomName,
                PlayersCount = r.GetPlayers().Length,
            } ).ToList();

            dto.Rooms = rooms;

            return dto;
        }

        public PlayerDTO GetPlayersInRoom(int roomID)
        {
            if (_rooms.TryGetValue(roomId, out Room room))
            {
                var players = room.GetPlayers();

                PlayerDTO player = new PlayerDTO();
                player.RoomId = roomID;
                player.Message = "Players retrieved successfully";
                player.Success = true;

                player.Players = players.Select(p => new Player()
                {
                    ID = p._id,
                    Name = p.GetEndpoint().ToString(),
                    LVL = 0
                }).ToList();


                return player;
            }
            else
            {
                Console.WriteLine($"Room with ID {roomId} not found.");
                PlayerDTO player = new PlayerDTO();
                player.RoomId = roomID;
                player.Message = "Room not found";
                player.Success = false;
                return player;
            }
        }

    }
}
