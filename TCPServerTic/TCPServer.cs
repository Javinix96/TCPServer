using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TCPServerTic.Interfaces;

namespace TCPServerTic
{
    public class TCPServer
    {
        private IServerManager _instance;
        private IRoomManager _roomManager;
        private TcpListener listener = null;
        private bool _running = true;

        public int port { private set; get; }
        public IPAddress ip { private set; get; }

        public TCPServer(IPAddress ip, int port, IServerManager sm, IRoomManager manager)
        {
            this.port = port;
            this.ip = ip;
            _instance = sm;
            _roomManager = manager;
        }

        public async Task Start()
        {
            listener = new TcpListener(ip, port);
            listener.Start();

            Console.WriteLine($"Server listening on {ip}:{port}");
            try
            {
                while (_running)
                {
                    Console.WriteLine("Esperando clientes");
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run( () => HandleClient(client) );

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                CloseServer();
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            Console.WriteLine("Cliente Conectado");
            var session =  _instance.AddClient(client,_roomManager);

            try 
            {
                session.SendWelcome("Holis");
                while (true)
                {
                    var readTask = session._stream.ReadAsync(session._buffer, 0, session._buffer.Length);
                     int bytes = await readTask;
                    if (bytes <= 0)
                    {
                        Console.WriteLine("0 bytes recibidos");
                        break;
                    }
                    session.bytesRead = bytes;
                    session.ReceiveData();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine("Cliente desconectado: " + e);
            }
            finally
            {
                session.Close();
                _instance.RemoveClient(session); 
            }
        }


        private void CloseServer()
        {
            listener.Dispose();
            listener.Stop();
            _running = false;
        }
    }
}
