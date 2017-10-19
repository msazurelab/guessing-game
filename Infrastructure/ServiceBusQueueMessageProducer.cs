using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ServiceBusQueueMessageProducer
    {
        public static void SendMessageToQueue(string connectionString, string queueName, object messageBody, string sessionId = null)
        {
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var jsonMessage = JsonConvert.SerializeObject(messageBody);
            var message = new BrokeredMessage(jsonMessage);
            if(!string.IsNullOrWhiteSpace(sessionId))
            {
                message.SessionId = sessionId;
            }
            client.Send(message);
        }
    }
}
