using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerTic.Clases.DTOS
{
    public class RoomInfoDTO
    {
        public int Count { set; get; }
        public List<RoomInfo>? Rooms { set; get; }
    }

    public class RoomInfo()
    {

        public int RoomId { set; get; }
        public string RoomName { set; get; }
        public int PlayersCount { set; get; } = 0;
    }
}
