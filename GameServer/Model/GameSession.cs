using GameServer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class GameSession : EntityBase
    {
        public Game Game { get; set; }

        public GameSession(string sessionId, Game game)
        {
            Id = sessionId;
            Game = game;
        }
    }
}
