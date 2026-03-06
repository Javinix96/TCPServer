using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PAcketRecieve
{
    internal class Welcome : IPacketHandler
    {
        public int Header => (int)PacketTypeSend.Welcome;

        public void Handle(ClientSession client, Packet payload)
        {
            string message = payload.ReadString();
            Console.WriteLine($"El cliente { client._id }: ha dicho {message}");
        }
    }
}
