using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class ReceiveMessage : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.Welcome;

        public void Handle(ClientSession client, Packet payload)
        {
            string message = payload.ReadString();
            Console.WriteLine($"El cliente {client._id}: ha dicho {message}");
        }

    }
}
