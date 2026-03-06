namespace TCPServerTic.Enums
{
    public enum PacketTypeReceive
    {
        Error = 0,
        Welcome,
        Message,
        RequestRooms,
        createRoom,
        Players,
    }
    public enum PacketTypeSend
    {
        Error = 999,
        Welcome,
        Message,
        RoomList,
        PlayersInRoom,
    }
    


}
