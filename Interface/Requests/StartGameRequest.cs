using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Requests
{
    public class StartGameRequest : Request
    {
        public string SessionId { get; set; }
        public StartGameRequest()
        {
            Type = RequestType.StartGame;
        }
    }
}
