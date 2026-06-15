
using System.Collections.Concurrent;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases
{
    public class Room : IRoom
    {
        private ConcurrentDictionary<int, ClientSession> _roomPlayers = new();
        private int _roomID;
        private string _roomName;
        private string _roomHost;
        private string _password;
        private bool _isPrivate;
        private int _time;
        private string currentTurn;
        private string board;


        public string RoomName => _roomName;
        public int RoomID => _roomID;
        public string RoomHost => _roomHost;
        public bool IsPrivate => _isPrivate;
        public string Password => _password;
        
        public bool HasPassword => !string.IsNullOrEmpty(_password);

        public Room(int roomID, string roomName,string hostname, string password, bool isPrivate, int time)
        {
            _roomPlayers = new ConcurrentDictionary<int, ClientSession>(2, 2);
            _roomID = roomID;
            _roomName = roomName;
            _roomHost = hostname;
            _password = password;
            _isPrivate = isPrivate;
            _time = time;

            board = "---------";
        }

        public bool AddPlayer(ClientSession player)
        {
            if (!TryAddPlayer(player.PlayerData.ID, player))
                return false;
            player.PlayerData.RoomID = _roomID;
            return true;

        }

        public int CountPlayers() => _roomPlayers.Count;

        public void RemovePlayer(ClientSession player)
        {
            if (player == null)
                return;

            _roomPlayers.TryRemove(player.PlayerData.ID, out _);
        }

        public async Task SendDataToPlayers() => await Task.Delay(500);


        private bool TryAddPlayer(int id, ClientSession session)
        {
            if (!_roomPlayers.TryAdd(id, session))
                return false;

            if (_roomPlayers.Count > 2)
            {
                _roomPlayers.TryRemove(id, out _);
                return false;
            }
            return true;
        }

        public ClientSession[] GetPlayers() => _roomPlayers.Values.ToArray();

        public void SetReadyPlayerByID(int id)
        {
            var players = GetPlayers();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].PlayerData.ID == id)
                {
                    players[i].PlayerData.Ready = true;
                    break;
                }
            }
        }


        public void SetPlayerInGame(int id)
        {
            var players = GetPlayers();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].PlayerData.ID == id)
                {
                    players[i].PlayerData.InGame = true;
                    break;
                }
            }
        }

        public void SendWho()
        {
            var players = GetPlayers();

            Random rr = new Random();
            string who = rr.Next(players.Length) == 0 ? "X" : "O";

            if (players.Length > 0)
            {
                if (!string.IsNullOrEmpty(players[0].PlayerData.Who))
                {
                    if (players[0].PlayerData.Who.Equals("X"))
                        players[1].PlayerData.Who = players[0].PlayerData.Who == "X" ? "O" : "X";
                }
                else
                {
                    players[0].PlayerData.Who = who;
                    players[1].PlayerData.Who = players[0].PlayerData.Who == "X" ? "O" : "X";
                }
            }

            for (int i = 0; i < players.Length; i++)
            {
                ClientSession pl = players[i];
                if (string.IsNullOrEmpty(pl.PlayerData.Who)) continue;

                pl.SendData(PacketFactory.CreateString(PacketTypeSend.SendWho, pl.PlayerData.Who));
            }
        }

        public void Decide()
        {
            Random random = new Random();
            currentTurn = random.Next(2) == 0 ? "X" : "O";
            SendDataPlayersInRoom(PacketFactory.CreateString(PacketTypeSend.SendTurn, currentTurn));
        }

        public int PlayerInGame()
        {
            int allPlayers = 0;
            foreach (var player in _roomPlayers.Values)
            {
                if (player.PlayerData.Ready)
                    allPlayers++;
            }

            return allPlayers;
        }

        public void SendDataPlayersInRoom(Packet pck)
        {
            foreach (var player in _roomPlayers.Values)
                player.SendData(pck);
        }

        public void RoomPosition(string player, int index)
        {
            if (!currentTurn.Equals(player)) return;

            char pos = board[index];

            if (pos != '-') return;

            var players = GetPlayers();

            foreach (var pl in players)
            {
                if (!pl.PlayerData.InGame) break;
                pl.SendData(PacketFactory.SendPos(PacketTypeSend.SendPosition, player, index));
            }
        }

        public void UpdateBoard(string player, int index)
        {
            var players = GetPlayers();
            bool ready = true;

            foreach (var pl in players)
            {
                if (pl.PlayerData.InGame) continue;

                ready = false;
                break;
            }

            if (!ready) return;


            if (!currentTurn.Equals(player)) return;

            char pos = board[index];

            if (pos != '-') return;

            var boardArr = board.ToCharArray();
            boardArr[index] = player[0];
            board = new string(boardArr);

            foreach (var pl in players)
                pl.SendData(PacketFactory.CreateString(PacketTypeSend.SendBoard, board));

            char turn = currentTurn.ToCharArray()[0];
            
            if (HasAWinner(turn))
            {
                var winner = GetWinner(turn);

                if (winner == null) return;

                winner.SendData(PacketFactory.CreateString(PacketTypeSend.SendWinner,$"Has ganado la partida {turn}"));
                
                var loser = GetLoser(winner.PlayerData.Who[0]);

                if (loser == null) return;

                loser.SendData(PacketFactory.CreateString(PacketTypeSend.SendWinner, $"Has Perdido la partida, suerte para la proxima {loser.PlayerData.Who}"));

                return;
            }

            ChangeTurn();

            SendDataPlayersInRoom(PacketFactory.CreateString(PacketTypeSend.SendTurn, $"Turno del jugador: {currentTurn}"));
        }

        public void ChoosePlayer()
        {
            Random r = new Random();

            int rand = r.Next(0, 100);

            if (CountPlayers() == 1)
            {
                _roomPlayers.Values.First().PlayerData.Who = rand % 2 == 0 ? "X" : "O";
                return;
            }

            var player1 = _roomPlayers.Values.ElementAt(0).PlayerData.Who;
            _roomPlayers.Values.ElementAt(1).PlayerData.Who = player1 == "X" ? "O" : "X";
        }


        private void ChangeTurn() => currentTurn = currentTurn == "X" ? "O" : "X";


        private bool HasAWinner(char turn)
        {
            int[,] checks = new int[8, 3]
            {
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 },
                { 0, 3, 6 },
                { 1, 4, 7 },
                { 2, 5, 8 },
                { 0, 5, 8 },
                { 2, 5, 6 },
            };

            for (int boardIndex = 0; boardIndex < 8; boardIndex++)
            {
                char c1 = board[checks[boardIndex, 0]];
                char c2 = board[checks[boardIndex, 1]];
                char c3 = board[checks[boardIndex, 2]];
                if (board[checks[boardIndex, 0]] == turn &&
                    board[checks[boardIndex, 1]] == turn &&
                    board[checks[boardIndex, 2]] == turn) return true;

            }
            return false;
        }
    
        private ClientSession GetWinner(char winner)
        {
            var players = GetPlayers();

            if (players.Length == 0) return null;

            if (players.Length == 1) return players[0];

            for(int playerIndex = 0; playerIndex < players.Length; playerIndex++)
                if (players[playerIndex].PlayerData.Who[0] == winner) return players[playerIndex];

            return null;
        }

        private ClientSession GetLoser(char winner)
        {
            var players = GetPlayers();

            if (players.Length == 0) return null;

            if (players.Length == 1) return null;

            for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
                if (players[playerIndex].PlayerData.Who[0] != winner) return players[playerIndex];

            return null;
        }
    }
}
