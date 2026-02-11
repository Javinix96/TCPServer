using System.Net;
using System.Text;
using TCPServerTic.Clases;

namespace TCPServerTic
{
    internal class Program
    {

        static void Main(string[] args)
        {
            ServerManager.SM.Init();
            TCPServer server = new(IPAddress.Any, 7777);
            _ = server.Start();
            Console.ReadKey();
        }
    }

}