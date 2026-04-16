using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using TCPServerTic.Clases.DTOS;
using TCPServerTic.Enums;
using TCPServerTic.Interfaces;

namespace TCPServerTic.Clases.PacketRecieve
{
    public class RoomCreate : IPacketHandler
    {
        public int Header => (int)PacketTypeReceive.ReceivedcreateRoom;
        private RoomManager _roomManager;

        public RoomCreate(RoomManager roomManager) => _roomManager = roomManager;
        

        public async void Handle(ClientSession client, Packet payload)
        {
            string nameRoom = payload.ReadString();
            bool isPrivate = payload.ReadBool();
            string passwordRoom = payload.ReadString();
            int time = payload.ReadInt();

            Console.WriteLine("Creando el cuarto " + nameRoom + " es privada: " + isPrivate + " contrasena: " + passwordRoom + " Tiempo " + time);
            await _roomManager.CreateRoom(client, nameRoom,passwordRoom,isPrivate,time);
        }
    }
}
