using System.Net.Sockets;


namespace TCPServerTic.Clases
{
    public class ClientSession
    {
        private TcpClient _tcpClient = null;
        private List<byte> _dataBuffer = new List<byte>();
        private Packet _pck;
        private int bufferLength = 0;
        private int _step = 0;

        public NetworkStream _stream = null;
        public int _id = 0;
        public byte[] _buffer = null;
        public int bytesRead = 0;
        public int bytesTotalRead = 0;

        public int Step { get => _step; private set => _step = value; }


        public ClientSession(int id,TcpClient client )
        {
            _tcpClient = client;
            _id = id;
            _stream = client.GetStream();
            _buffer = new byte[1024]; 
            _dataBuffer = new List<byte>();
            _step = 0;
            _pck = new Packet();
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
                _pck.Dispose();


            while (lengthData > 0 && lengthData <= _pck.UnreadLength())
            {
                var data2 = _pck.ReadBytes(lengthData);
                using (Packet _pck = new Packet(data2))
                {
                    //Recibimos los paquetes y hacermos la logica
                    int id = _pck.ReadInt();
                    string message = _pck.ReadString();

                    Console.WriteLine($"El cliente({id}) dice: { message}");
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

                _pck.Dispose();
                ClearBuffer();
                break;
            }

        }

        public void SendWelcome( string message)
        {
            if (_stream == null)
                return;

            try
            {

                using (Packet _pck = new Packet())
                {
                    _pck.WriteInt(1);
                    _pck.WriteInt(this._id);
                    _pck.WriteString(message);
                    _pck.WriteLength();

                    SendToCLient(_pck);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al enviar el mensaje de bienvenida: " + e.ToString());
            }
        }

        private void SendToCLient(Packet _pck)
        {
            if (_stream == null)
                return;

            _stream.Write(_pck.GetBytesArray(), 0, _pck.GetBytesArray().Length);
        }

        private void ClearBuffer()
        {
            _buffer = new byte[0];
            bytesRead = 0;
            bufferLength = 0;
            _dataBuffer.Clear();
            _step = 0;
        }

        public void Close()
        {
            ServerManager.SM.RemoveClient(this);
            _tcpClient.Dispose();
            _tcpClient.Close();
        }
    }
}
