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

            var roomsdto = GetRooms();
            
            var ss = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.RoomList, roomsdto);
            
            _serverManager.SendToAll(ss);


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
            Room room = null;
            PlayerDTO player = new PlayerDTO();
            if (!_rooms.TryGetValue(roomId, out room))
            {
                player.Success = false;
                player.RoomId = roomId;
                player.Message = "Error en unirse al server";
                return player;
            }

            var players = room.GetPlayers();
            player.RoomId = roomID;
            player.Message = "Players retrieved successfully";
            player.Success = true;
            player.Players = players.Select(p => new Player()
            {
                ID = p._id,
                Name = $"Player {p._id}",
                LVL = 0
            }).ToList();


            return player;

        }

        public PlayerDTO OnPlayerJoin(int roomId, ClientSession client)
        {
            Room room = null;
            PlayerDTO player = new PlayerDTO();

            if (!_rooms.TryGetValue(roomId,out room))
            {
                player.Success=false;
                player.RoomId = roomId;
                player.Message = "Error en unirse al server";
                return player;
            }

            if (!room.AddPlayer(client))
            {
                player.Success = false;
                player.RoomId = roomId;
                player.Message = "Error en agregar al jugador";
                return player;
            }

            return GetPlayersInRoom(roomId);
        }

        public void SendPlayerInARoom(int roomID,Packet pck)
        {
            Room room = null;
            if (!_rooms.TryGetValue(roomID, out room)) return;

            foreach(ClientSession session in room.GetPlayers())
                    session.SendData(pck);

        }
    
        public void RemovePlayerFromRoom(ClientSession session)
        {
            _rooms.TryRemove(session._id,out _);

            //if (_rooms.player)
        }
    }
}
