using System.Net;
using System.Net.Sockets;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;
using TCPServerTic.Routers;


namespace TCPServerTic.Clases
{
    public class ClientSession
    {
        public TcpClient _tcpClient = null;
        private List<byte> _dataBuffer = new List<byte>();
        private Packet _pck;
        private IServerManager _sm = null;
        private IRoomManager _roomManager;
        private PacketRouter _router = null;    

        public NetworkStream _stream = null;
        public int _id = 0;
        public byte[] _buffer = null;
        public int bytesRead = 0;
        public int bytesTotalRead = 0;

        public ClientSession(int id,TcpClient client,IServerManager sm, PacketRouter router,IRoomManager rm)
        {
            _tcpClient = client;
            _id = id;
            _stream = client.GetStream();
            _buffer = new byte[1024]; 
            _dataBuffer = new List<byte>();
            _pck = new Packet();
            _sm = sm;
            _router = router;
            _roomManager = rm;
        }


        public void ReceiveData()
        {
            bytesTotalRead += bytesRead;
            byte[] data = new byte[bytesRead];

            Array.Copy(_buffer, data, bytesRead);
            _pck.SetBytes(data);

            if (_pck.GetBytesArray().Length < 4)
                return;

            int lengthData = _pck.ReadInt();

            if (lengthData <= 0)
                return;

            while (lengthData > 0 && lengthData <= _pck.UnreadLength())
            {
                var data2 = _pck.ReadBytes(lengthData);
                using (Packet _pck = new Packet(data2))
                {
                    int id = _pck.ReadInt();
                    Packet pckTemp = _pck.Copy();
                    _router.Route((byte)id, _pck, this);
                }

                if (_pck.UnreadLength() >= 4)
                {
                    lengthData = _pck.ReadInt();
                    if (lengthData <= 0)
                    {
                        ClearBuffer();
                        break;
                    }
                    continue;
                }
                ClearBuffer();
                break;
            }
        }

        public void SendWelcome(string message)
        {
            if (_stream == null)
            {
                Console.WriteLine("nulo");
                return;
            }

            try
            {
                using (Packet _pck = new Packet())
                {
                    _pck.WriteInt((int)PacketTypeSend.Welcome);
                    _pck.WriteInt(this._id);
                    _pck.WriteString(message);
                    _pck.WriteLength();

                    SendToClient(_pck);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al enviar el mensaje de bienvenida: " + e.ToString());
            }
        }

        public void SendData(Packet _pck)
        {
            if (_stream == null)
                return;
            try
            {
                SendToClient(_pck);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al enviar datos al cliente: " + e.ToString());
            }
        }

        private void SendToClient(Packet _pck)
        {
            if (_stream == null)
                return;

            _stream.Write(_pck.GetBytesArray(), 0, _pck.GetBytesArray().Length);
        }

        private void ClearBuffer()
        {
            _pck = new Packet();
            _buffer = new byte[1024];
            bytesRead = 0;
            _dataBuffer.Clear();
        }

        public IPEndPoint GetEndpoint()
        {
            if (_tcpClient.Client.RemoteEndPoint is IPEndPoint remoteEndPoint)
            {
                return remoteEndPoint;
            }
            return null;
        }

        public void Close()
        {
            _tcpClient.Dispose();
            _tcpClient.Close();
            _roomManager.RemovePlayerFromRoom(this);
        }
    }
}
