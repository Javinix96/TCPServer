using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCPServerTic.Clases;

namespace TCPServerTic
{
    public class TCPServer
    {
        private TcpListener listener = null;
        private bool _running = true;

        public int port { private set; get; }
        public IPAddress ip { private set; get; }

        public TCPServer(IPAddress ip, int port)
        {
            this.port = port;
            this.ip = ip;
            listener = new TcpListener(ip, port);
        }

        public async Task Start()
        {
            listener.Start();
            try
            {
                while (_running)
                {
                    Console.WriteLine("Esperando clientes");
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Cliente Conectado");

                    _ = HandleClient(client);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                CloseServer();
            }
            finally
            {
                CloseServer();
            }
        }


        private async Task HandleClient(TcpClient client)
        {
            var session = ServerManager.SM.AddClient(client);

            try 
            { 
                session.SendWelcome("Bienvenido al servidor TCP!");

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
                Console.WriteLine("Cliente desconectado");
            }
        }


        private void CloseServer()
        {
            listener.Dispose();
            listener.Stop();
        }
    }
}
