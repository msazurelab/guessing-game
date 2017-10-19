using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClient.Application;

namespace GameClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new GameClientApp();
            app.Run();
        }
    }
}
