
namespace TCPServerTic.Clases
{
    public class Player
    {
        public bool Play { get; set;  }
        public int ID { get; set; } = 0;
        public int RoomID { get; set; } = 0;
        public string Name { get; set; }
        public bool InGame { get; set; } = false;
        public string Who { get; set; }
    }
}
