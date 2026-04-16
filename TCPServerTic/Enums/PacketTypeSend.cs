namespace TCPServerTic.Enums
{
    public enum PacketTypeReceive
    {
        ReceivedError = 0,
        ReceivedWelcome,
        ReceivedExit,
        ReceivedMessage,
        ReceivedRequestRooms,
        ReceivedcreateRoom,
        ReceivedPlayers,
        ReceivedJoinRoomRequest,
        ReceivedExitRoom,
        ReceivedRequestJoin,
        ReceivedPlayerReady,
        ReceivedPosition,
        ReceivedReadyPos
    }
    public enum PacketTypeSend
    {
        SendError = 999,
        SendAccept,
        SendWelcome,
        SendMessage,
        SendRoomList,
        SendRoomCreated,
        SendPlayersInRoom,
        SendJoinRoom,
        SendLoadScene,
        SendCounter,
        SendWho,
        SendTurn,
        SendPosition,
        SendBoard,
        SendWinner
    }
    


}
