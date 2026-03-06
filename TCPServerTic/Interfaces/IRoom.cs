using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerTic.Clases;

namespace TCPServerTic.Interfaces
{
    public interface IRoom
    {
        void AddPlayer(ClientSession player);
        void RemovePlayer(ClientSession player);
        Task SendDataToPlayers();

        ClientSession[] GetPlayers();
    }
}
