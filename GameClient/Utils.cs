using Interface;
using Interface.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    public static class Utils
    {
        public static void DisplayQuestions(Question[] questions)
        {
            foreach (var question in questions)
            {
                Console.WriteLine($"{question.Id} - {question.Value}");
            }
        }
    }
}
