using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Responses
{
    public class StartGameResponse : Response
    {
        public Question[] Questions { get; set; }
        public StartGameResponse()
        {
            Type = ResponseType.StartGame;
        }
    }
}
