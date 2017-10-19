using Interface;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Deployment
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationSettings.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            SetupQueue(connectionString);
        }

        public static void SetupQueue(string connectionString)
        {
            NamespaceManager nameSpaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            var queueDescription = new QueueDescription(ApplicationConstants.RequestQueueName);
            queueDescription.SupportOrdering = true;
            nameSpaceManager.CreateQueue(queueDescription);

            var responseDescription = new QueueDescription(ApplicationConstants.ResponseQueueName);
            responseDescription.SupportOrdering = true;
            responseDescription.RequiresSession = true;
            nameSpaceManager.CreateQueue(responseDescription);
        }
    }
}
