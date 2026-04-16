using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PAcketRecieve
{
    internal class Welcome : IPacketHandler
    {
        public int Header => (int)PacketTypeSend.SendWelcome;

        public void Handle(ClientSession client, Packet payload)
        {
            string message = payload.ReadString();
            Console.WriteLine($"El cliente { client.PlayerData.ID}: ha dicho {message}");
        }
    }
}
