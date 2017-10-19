using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Requests
{
    public class AskQuestionRequest : Request
    {
        public int QuestionId { get; set; }
        public AskQuestionRequest(int questionId)
        {
            QuestionId = questionId;
            Type = RequestType.AskQuestion;
        }
    }
}
