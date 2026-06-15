using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.Json;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;
using static System.Collections.Specialized.BitVector32;

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
            dto.Room = new RoomInfo();

            Room newRoom = new Room(newRoomId,name,session.PlayerData.Name, password, isPrivate, time);

            if (!_rooms.TryAdd(newRoomId, newRoom))
            {
                Console.WriteLine($"Failed to create room with ID {newRoomId}");
                dto.Success = false;
                dto.Message = "Error on createrd a room";
                Console.WriteLine("Hubo un error" + name);
                return null;
            }

            newRoom.AddPlayer(session);
            newRoom.ChoosePlayer();

            dto.Room.RoomId = newRoomId;
            dto.Room.RoomName = name;

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
            var roomstmp = (from rs in _rooms where rs.Value.IsPrivate == false select rs.Value).ToList();
            var rooms = roomstmp.Select(r => new RoomInfo
            {
                RoomId = r.RoomID,
                RoomName = r.RoomName,
                RoomHost = r.RoomHost,
                PlayersCount = r.GetPlayers().Length,
            } ).ToList();

            dto.Rooms = rooms;

            return dto;
        }

        public void SearchRoom(ClientSession session, string roomName)
        {
            Room room = null;
            int roomID = SearchRoomByName(roomName);

            if (roomID == -1)
            {
                using (Packet pck = new Packet())
                {
                    pck.WriteInt((int)PacketTypeSend.SendMessage);
                    pck.WriteString("No se encontro la sala");
                    pck.WriteLength();
                    session.SendData(pck);
                }
                return;
            }

            if (!_rooms.TryGetValue(roomID, out room))
            {
                using (Packet pck = new Packet())
                {
                    pck.WriteInt((int)PacketTypeSend.SendMessage);
                    pck.WriteString($"No se encontro la sala {roomID}");
                    pck.WriteLength();
                    session.SendData(pck);
                }
                return;
            }

            RoomInfoDTO dto = new RoomInfoDTO();

            dto.Count = 1;
            dto.Rooms = new List<RoomInfo>()
            {
                new RoomInfo()
                {
                    RoomId = room.RoomID,
                    RoomName = room.RoomName,
                    RoomHost = room.RoomHost,
                    PlayersCount = room.GetPlayers().Length
                }
            };

            using (Packet pck = new Packet())
            {
                pck.WriteInt((int)PacketTypeSend.SendSearchedRoom);
                pck.WriteString(JsonConvert.SerializeObject(dto));
                pck.WriteLength();
                session.SendData(pck);
            }
        }

        private int SearchRoomByName(string nameRoom)
        {
            var room = _rooms.Values.FirstOrDefault(r => r.RoomName.Equals(nameRoom, StringComparison.OrdinalIgnoreCase));
            return room != null ? room.RoomID : -1;
        }

        public PlayerDTO GetPlayersInRoom(int roomID)
        {
            Room room = null;
            PlayerDTO player = new PlayerDTO();
            player.Room = new RoomInfo();
            if (!_rooms.TryGetValue(roomId, out room))
            {
                player.Success = false;
                player.RoomId = roomId;
                player.Message = "Error en unirse al server";
                return player;
            }

            var players = room.GetPlayers();
            player.RoomId = roomID;
            player.Room.RoomName = room.RoomName;
            player.Room.RoomId = roomID;
            player.Message = "Players retrieved successfully";
            player.Success = true;
            player.Players = players.Select(p => new PlayerInfoDTO()
            {
                ID = p.PlayerData.ID,
                Name = p.PlayerData.Name,
                LVL = 0,
                Ready = p.PlayerData.Ready,
                Who = p.PlayerData.Who
            }).ToList();

            player.RoomHasPassword = false;

            return player;

        }

        public PlayerDTO OnPlayerJoin(int roomId, ClientSession client, bool userWrotePass = false, string password = "")
        {
            Room room = null;
            PlayerDTO player = new PlayerDTO();
            player.RoomId = roomId;

            if (!_rooms.TryGetValue(roomId,out room))
            {
                player.Success=false;
                player.Message = "Error en unirse a la sala";
                return player;
            }

            player.RoomName = room.RoomName;

            if (room.HasPassword)
            {
                if (userWrotePass)
                {
                    if (room.Password != password)
                    {
                        player.Success = false;
                        player.Message = "Contraseña incorrecta";
                        return player;
                    }
                }
                else
                {
                    player.Success = true;
                    player.RoomHasPassword = true;
                    player.Message = "Se requiere contraseña";
                    using (Packet pck = new Packet())
                    {
                        pck.WriteInt((int)PacketTypeSend.SendRequirePassword);
                        pck.WriteString(JsonConvert.SerializeObject(player));
                        pck.WriteLength();
                        client.SendData(pck);
                    }
                    return null;
                }
            }

            if (!room.AddPlayer(client))
            {
                player.Success = false;
                player.RoomId = roomId;
                player.Message = "Error en agregar al jugador";
                return player;
            }

            room.ChoosePlayer();

            var roomsdto = GetRooms();
            var ss = PacketFactory.Create<RoomInfoDTO>(PacketTypeSend.SendRoomList, roomsdto);

            _serverManager.SendToAll(ss);

            client.PlayerData.RoomID = roomId;
    
            return GetPlayersInRoom(roomId);
        }

        public void SendPlayersToARoom(int roomID,Packet pck, int idSended = 0)
        {
            Room room = null;
            if (!_rooms.TryGetValue(roomID, out room)) return;

            foreach(ClientSession session in room.GetPlayers())
                if (idSended != session.PlayerData.ID )
                    session.SendData(pck);

        }
    
        public PlayerDTO RemovePlayerFromRoom(int roomID,ClientSession session, int times = 1)
        {
            PlayerDTO player = new PlayerDTO();
            player.Room = new RoomInfo();
            Room room = null;

            player.Success = false;
            if (!_rooms.TryGetValue(roomID, out room))  return player;

            player.Success = true;
            player.Room.RoomName = room.RoomName;
            player.Room.RoomId = roomId;
            player.Room.RoomHost = room.RoomHost;
            room.RemovePlayer(session);


            player.RoomId = roomID;
            player.Players = GetPlayers(room);
            
            using (Packet pck = new Packet())
            {
                pck.WriteInt((int)PacketTypeSend.sendExitRoom);
                pck.WriteInt(times);
                pck.WriteLength();
                session.SendData(pck);
            }

            var pck2 = PacketFactory.Create<PlayerDTO>(PacketTypeSend.SendPlayersInRoom, player);
            SendDataAllPlayerInRoom(room,pck2);

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

        public void SetReadyPlayer(int roomID, int plID, ClientSession session)
        {
            Room room = null;
            Packet pck = new Packet();

            if (!_rooms.TryGetValue(roomID, out room)) return;

            room.SetReadyPlayerByID(plID);

            //room.SetPlayerInGame(session.PlayerData.ID);
            using (Packet pck2 = new Packet())
            {
                pck2.WriteInt((int)PacketTypeSend.SendPlayerReady);
                pck2.WriteInt(plID);
                pck2.WriteLength();
                room.SendDataPlayersInRoom(pck2);
            }

            int playersInGame = room.PlayerInGame();

            if (playersInGame == 2)
            {
                pck = PacketFactory.CreateString(PacketTypeSend.SendMessage, "Empezando partida.....");
                room.SendDataPlayersInRoom(pck);
                SendTimer(room);
                //room.SendWho();
                //room.Decide();
                return;
            }

            pck = PacketFactory.CreateString(PacketTypeSend.SendMessage, "Esperando que den listo los demas jugadores");
            session.SendData(pck);
        }

        private List<PlayerInfoDTO> GetPlayers(Room room)
        {

            var players = room.GetPlayers();

            return players.Select(p => new PlayerInfoDTO()
            {
                ID = p.PlayerData.ID,
                Name = p.PlayerData.Name,
                LVL = 0
            }).ToList();
        }

        private void SendTimer(Room room)
        {
            int time =  0;
            foreach (var player in room.GetPlayers())
                _ = SendTimer(player);
            
        }

        private async Task SendTimer(ClientSession session)
        {
            int count = 10;
            bool joined = false;

            while (count >= 0)
            {
                Packet pck = PacketFactory.SendInt(PacketTypeSend.SendCounter,count);
                session.SendData(pck);
                await Task.Delay(1000);
                count--;
            }
            
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

        private void SendDataAllPlayerInRoom(Room room,Packet pck)
        {
            var players = room.GetPlayers();
            foreach(var player in players)
                player.SendData(pck);

        }
    }
}
