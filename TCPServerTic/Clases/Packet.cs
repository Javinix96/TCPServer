using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerTic.Clases
{

    public class Packet : IDisposable
    {

        private List<byte> bufferList = null;
        private byte[] bufferArray = null;
        public int readPos = 0;
        private bool disposed = false;

        public Packet()
        {
            bufferList = new List<byte>();
            readPos = 0;
        }

        public Packet(byte[] data)
        {
            bufferList = new List<byte>();
            readPos = 0;
            WriteBytes(data);
            bufferArray = bufferList.ToArray();
        }
        public Packet Copy()
        {
            Packet pck = new Packet();
            pck.bufferArray = new byte[bufferArray.Length];
            Array.Copy(bufferArray, pck.bufferArray, bufferArray.Length);
            pck.readPos = this.readPos;
            pck.bufferList = new List<byte>(bufferList);

            return pck;
        }


        public byte[] GetBytesArray()
        {
            bufferArray = bufferList.ToArray();
            return bufferArray;
        }

        public void SetBytes(byte[] data)
        {
            WriteBytes(data);
            bufferArray = bufferList.ToArray();
        }

        public void WriteBytes(byte[] value)
        {
            bufferList.AddRange(value);
        }

        public void WriteInt(int value)
        {
            bufferList.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteFloat(float value)
        {
            bufferList.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteBool(bool value)
        {
            bufferList.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteString(string value)
        {
            WriteInt(value.Length);
            bufferList.AddRange(Encoding.ASCII.GetBytes(value));
        }

        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (bufferList.Count > readPos)
            {
                byte[] value = bufferList.GetRange(readPos, length).ToArray();
                if (moveReadPos)
                {
                    readPos += length;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read the value.");
            }
        }

        public int ReadInt(bool moveReadPos = true)
        {
            if (bufferList.Count > readPos)
            {
                int value = BitConverter.ToInt32(bufferArray, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read the value.");
            }
        }

        public float ReadFloat(bool moveReadPos = true)
        {
            if (bufferList.Count > readPos)
            {
                float value = BitConverter.ToSingle(bufferArray, readPos);
                if (moveReadPos)
                {
                    readPos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read the value.");
            }
        }

        public bool ReadBool(bool moveReadPos = true)
        {
            if (bufferList.Count > readPos)
            {
                bool value = BitConverter.ToBoolean(bufferArray, readPos);
                if (moveReadPos)
                {
                    readPos += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Could not read the value.");
            }
        }

        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(bufferArray, readPos, length);
                if (moveReadPos && value.Length > 0)
                {
                    readPos += length;
                }
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public void WriteLength() => bufferList.InsertRange(0, BitConverter.GetBytes(bufferList.Count));

        public int UnreadLength() => bufferArray.Length - readPos;

        public void Reset()
        {
            bufferList?.Clear();
            bufferList = new List<byte>();
            bufferArray = null;
            readPos = 0;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    bufferList?.Clear();
                    bufferList = null;
                    bufferArray = null;
                    readPos = 0;
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }

}
