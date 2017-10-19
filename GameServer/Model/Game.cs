using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Game
    {
        public List<Hint> Hints { get; set; }

        public string Solution { get; set; }
    }
}
