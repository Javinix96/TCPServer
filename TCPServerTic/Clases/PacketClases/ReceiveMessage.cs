using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class ReceiveMessage : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedWelcome;

        public void Handle(ClientSession client, Packet payload)
        {
            string message = payload.ReadString();
            Console.WriteLine($"El cliente {client.PlayerData.ID}: ha dicho {message}");
        }

    }
}
