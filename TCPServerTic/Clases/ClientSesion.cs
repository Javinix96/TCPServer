using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerTic.Clases
{
    public class ClientSesion
    {
        private TcpClient _tcpClient = null;
        public int _id = 0;
        public byte[] _buffer = null;
        public NetworkStream _stream = null;

        public int bytesRead = 0;


        public ClientSesion(int id,TcpClient client )
        {
            _tcpClient = client;
            _id = id;
            _stream = client.GetStream();
            _buffer = new byte[1024]; 
        }

        public void ProccessData()
        {
            if (_buffer == null)
                return;
            if (_buffer.Length <= 0)
                return;
            if (_stream == null)
                return;
            if (bytesRead <= 0)
                return;

            byte[] data = new byte[bytesRead];
            for(int i = 0; i < bytesRead; i++)
                data[i] = _buffer[i];
            
            using (Packet pck = new Packet(data))
            {
                Console.WriteLine("id: " + pck.ReadInt());
                Console.WriteLine("Lenght: " + pck.ReadInt(false));
                Console.WriteLine("string: " + pck.ReadString()); 
            }

            _buffer = new byte[1024];
        }

        public void Close()
        {
            ServerManager.SM.RemoveClient(this);
            _tcpClient.Dispose();
            _tcpClient.Close();
        }




    }
}
