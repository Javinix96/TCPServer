using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketClases
{
    public class ExitPlayer : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedExit;

        private ServerManager _serverManager;

        public ExitPlayer(ServerManager sm) => _serverManager = sm;

        public void Handle(ClientSession client, Packet payload)
        {
            _serverManager.Disconnect(client);
        }
    }
}
