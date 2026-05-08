namespace TCPServerTic.Enums
{
    public enum PacketTypeReceive
    {
        ReceivedError = 0,
        ReceivedWelcome,
        ReceivedExit,
        ReceivedMessage,
        ReceivedName,
        ReceivedRequestRooms,
        ReceivedcreateRoom,
        ReceivedPlayers,
        ReceivedJoinRoomRequest,
        ReceivedExitRoom,
        ReceivedRequestJoin,//game
        ReceivedPlayerReady,
        ReceivedSearchRoom,
        ReceivedPassword,
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
        SendRequestJoinToRoom,
        SendLoadScene,
        SendCounter,
        SendWho,
        sendExitRoom,
        SendPlayerReady,
        SendSearchedRoom,
        SendRequirePassword,
        SendTurn,
        SendPosition,
        SendBoard,
        SendWinner
    }
    


}
