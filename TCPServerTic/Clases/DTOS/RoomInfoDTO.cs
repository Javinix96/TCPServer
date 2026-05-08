
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
        public string? RoomName { set; get; }
        public string? RoomHost { set; get; }
        public int PlayersCount { set; get; } = 0;
    }
}
