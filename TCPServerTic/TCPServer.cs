using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
                //await SkipProxyHeader(session.Stream);
                while (session._tcpClient.Connected)
                {
                    
                    var readTask = session.Stream.ReadAsync(session._buffer, 0, session._buffer.Length);
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
        private async Task SkipProxyHeader(NetworkStream stream)
        {
            List<byte> header = new List<byte>();

            byte[] buffer = new byte[1];

            while (true)
            {
                int read = await stream.ReadAsync(buffer, 0, 1);
                if (read == 0) return;

                header.Add(buffer[0]);

                int count = header.Count;

                // detectar \r\n
                if (count >= 2 &&
                    header[count - 2] == '\r' &&
                    header[count - 1] == '\n')
                {
                    break;
                }
            }

            string headerStr = Encoding.ASCII.GetString(header.ToArray());
            Console.WriteLine($"Proxy header: {headerStr}");
        }



        private void CloseServer()
        {
            listener.Dispose();
            listener.Stop();
            _running = false;
        }
    }
}
