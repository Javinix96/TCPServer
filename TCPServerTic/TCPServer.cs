using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TCPServerTic.Interfaces;

namespace TCPServerTic
{
    public class TCPServer
    {
        private IServerManager _instance;
        private TcpListener listener = null;
        private bool _running = true;

        public int port { private set; get; }
        public IPAddress ip { private set; get; }

        public TCPServer(IPAddress ip, int port, IServerManager sm)
        {
            this.port = port;
            this.ip = ip;
            _instance = sm;
        }

        public async Task Start()
        {
            listener = new TcpListener(ip, port);
            listener.Start();
            try
            {
                while (_running)
                {
                    Console.WriteLine("Esperando clientes");
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = HandleClient(client);
                    Console.WriteLine("Cliente Conectado");
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
            var session = _instance.AddClient(client);

            try 
            {
                session.SendWelcome("Holiwis");
                while ((session.bytesRead = await session._stream.ReadAsync(session._buffer, 0, session._buffer.Length)) > 0)
                {
                    if (session.bytesRead <= 0)
                        break;

                    session.ReceiveData();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine("Cliente desconectado");
            }
            finally
            {
                session.Close();
                _instance.RemoveClient(session);
                Console.WriteLine("Cliente desconectado");
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
