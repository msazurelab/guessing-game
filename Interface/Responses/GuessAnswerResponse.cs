using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Responses
{
    public class GuessAnswerResponse : Response
    {
        public bool IsSolutionCorrect { get; set; }
        public GuessAnswerResponse()
        {
            Type = ResponseType.GuessAnswer;
        }
    }
}
