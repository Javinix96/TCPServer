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

        public async Task<PlayerDTO> CreateRoom(ClientSession session,string name, string password, bool isPrivate, int time)
        {
   
            int newRoomId = Interlocked.Increment(ref roomId);

            PlayerDTO dto = new PlayerDTO();

            Room newRoom = new Room(newRoomId,name,password, isPrivate, time);

            if (!_rooms.TryAdd(newRoomId, newRoom))
            {
                Console.WriteLine($"Failed to create room with ID {newRoomId}");
                dto.Success = false;
                dto.Message = "Error on createrd a room";
                Console.WriteLine("Hubo un error" + name);
                return null;
            }

            newRoom.AddPlayer(session);


            var roomsdto = GetRooms();
            
            var ss = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.SendRoomList, roomsdto);
            
            _serverManager.SendToAll(ss);

            dto = GetPlayersInRoom(newRoomId);

            var packet = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendRoomCreated, dto);
            session.SendData(packet);

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
            player.Players = players.Select(p => new PlayerInfoDTO()
            {
                ID = p.PlayerData.ID,
                Name = $"Player {p.PlayerData.ID}",
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

            client.PlayerData.RoomID = roomId;

            return GetPlayersInRoom(roomId);
        }

        public void SendPlayersToARoom(int roomID,Packet pck)
        {
            Room room = null;
            if (!_rooms.TryGetValue(roomID, out room)) return;

            foreach(ClientSession session in room.GetPlayers())
                    session.SendData(pck);

        }
    
        public PlayerDTO RemovePlayerFromRoom(int roomID,ClientSession session)
        {
            PlayerDTO player = new PlayerDTO();
            Room room = null;

            player.Success = false;
            if (!_rooms.TryGetValue(roomID, out room))  return player;

            player.Success = true;
            room.RemovePlayer(session);

            player.RoomId = roomID;
            player.Players = GetPlayers(room);

            
            if (room.GetPlayers().Length > 0) return player;

            _rooms.TryRemove(room.RoomID, out _);

            return player;
        }

        public bool CanLoadSceneGame(int clientID, int id)
        {
            Room room = null;

            if (!_rooms.TryGetValue(id, out room)) return false;

            room.SetReadyPlayerByID(clientID);

            SendTimer(room);

            return room.GetPlayers().Length == 2;
        }

        public void SetReadyPlayer(int roomID,ClientSession session)
        {
            Room room = null;
            Packet pck = new Packet();

            if (!_rooms.TryGetValue(roomID, out room)) return;

            room.SetPlayerInGame(session.PlayerData.ID);


            int playersInGame = room.PlayerInGame();
            if (playersInGame == 2)
            {
                pck = PacketFactory.CreateString(PacketTypeSend.SendMessage, "Empezando partida.....");
                room.SendDataPlayersInRoom(pck);
                room.SendWho();
                room.Decide();
                return;
            }
            
            pck = PacketFactory.CreateString(PacketTypeSend.SendMessage, "Esperando jugadores");
            session.SendData(pck);            
        }

        private List<PlayerInfoDTO> GetPlayers(Room room)
        {

            var players = room.GetPlayers();

            return players.Select(p => new PlayerInfoDTO()
            {
                ID = p.PlayerData.ID,
                Name = $"Player {p.PlayerData.ID}",
                LVL = 0
            }).ToList();
        }

        private void SendTimer(Room room)
        {
            int time =  0;
            foreach (var player in room.GetPlayers())
            {
                if (player.PlayerData.Play)
                    continue;
                _ = SendTimer(player);
            }
        }

        private async Task SendTimer(ClientSession session)
        {
            int count = 1;
            bool joined = false;

            while (count < 11)
            {
                if (joined) break;
                joined = session.PlayerData.InGame;
                Packet pck = PacketFactory.SendInt(PacketTypeSend.SendCounter,count);
                session.SendData(pck);
                await Task.Delay(1000);
                count++;
            }

            if (joined) return;
            
            Packet pck2 = PacketFactory.SendBool(PacketTypeSend.SendLoadScene,true);
            session.SendData(pck2);
        }
    
        public void RoomPosition(int roomID, string player, int index)
        {
            Room room;

            if (!_rooms.TryGetValue(roomID, out room))
                return;

            room.RoomPosition(player, index);
        }

        public void UpdateBoard(int roomID, string player, int index)
        {
            Room room;

            if (!_rooms.TryGetValue(roomID, out room))
                return;

            room.UpdateBoard(player,index);       
        }
    }
}
