using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Requests
{
    public class GuessAnswerRequest : Request
    {
        public string Answer { get; set; }
        public GuessAnswerRequest(string answer)
        {
            Type = RequestType.GuessAnswer;
            Answer = answer;
        }
    }
}
