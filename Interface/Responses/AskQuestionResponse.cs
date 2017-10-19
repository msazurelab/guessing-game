using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Responses
{
    public class AskQuestionResponse : Response
    {
        public bool IsTrue { get; set; }
        public AskQuestionResponse()
        {
            Type = ResponseType.AskQuestion;
        }
    }
}
