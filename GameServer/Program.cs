using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Application;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverApp = new GameServerApp();
            serverApp.Run();
        }
    }
}
