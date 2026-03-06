using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerTic.Clases.DTOS
{
    public class PlayerDTO
    {
        public int RoomId { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<Player> Players { get; set; }

    }

    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LVL { get; set; }
    }
}
