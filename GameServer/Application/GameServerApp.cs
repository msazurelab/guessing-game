using System;
using Microsoft.ServiceBus.Messaging;
using Interface;
using Newtonsoft.Json;
using Interface.Requests;
using Interface.Responses;
using Utils;
using System.Configuration;

namespace GameServer.Application
{
    public class GameServerApp
    {
        private string _connectionSTring;

        public GameServerApp()
        {
            _connectionSTring = ConfigurationSettings.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        }

        public void Run()
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionSTring, ApplicationConstants.RequestQueueName, ReceiveMode.PeekLock);

            while (true)
            {
                var message = queueClient.Receive();
                if (message == null)
                {
                    continue;
                }
                try
                {
                    string messageBody = message.GetBody<string>();
                    Console.WriteLine($"Received Message Id: {message.MessageId} Body: {messageBody}");
                    var requestBase = JsonConvert.DeserializeObject<Request>(messageBody);
                    switch (requestBase.Type)
                    {
                        case RequestType.StartGame:
                            HandleStartGameRequest(_connectionSTring, messageBody);
                            break;

                        case RequestType.AskQuestion:
                            HandleAskQuestionRequest(_connectionSTring, message, messageBody);
                            break;

                        case RequestType.GuessAnswer:
                            HandleGuessAnswerRequest(_connectionSTring, message, messageBody);
                            break;
                    }
                    message.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process message {message.MessageId} " + ex);
                    try
                    {
                        message.DeadLetter();
                    }
                    catch
                    // best effort to throw into dead letter, ignore if it failed
                    { }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void HandleStartGameRequest(string connectionString, string messageBody)
        {
            var request = JsonConvert.DeserializeObject<StartGameRequest>(messageBody);
            var gameSession = GameSessionManager.Instance.StartGameSession(request.SessionId);
            var response = new StartGameResponse()
            {
                Message = "Game Started!",
                Questions = GameSessionManager.Instance.GetQuestions()
            };
            ServiceBusQueueMessageProducer.SendMessageToQueue(connectionString, ApplicationConstants.ResponseQueueName, response, gameSession.Id);
        }

        private static void HandleAskQuestionRequest(string connectionString, BrokeredMessage message, string messageBody)
        {
            var request = JsonConvert.DeserializeObject<AskQuestionRequest>(messageBody);
            var isTrue = GameSessionManager.Instance.AnswerQuestion(message.SessionId, request.QuestionId);
            var response = new AskQuestionResponse()
            {
                IsTrue = isTrue
            };
            ServiceBusQueueMessageProducer.SendMessageToQueue(connectionString, ApplicationConstants.ResponseQueueName, response, message.SessionId);
        }

        private static void HandleGuessAnswerRequest(string connectionString, BrokeredMessage message, string messageBody)
        {
            var request = JsonConvert.DeserializeObject<GuessAnswerRequest>(messageBody);
            var correct = GameSessionManager.Instance.GuessAnswer(message.SessionId, request.Answer);
            var response = new GuessAnswerResponse()
            {
                IsSolutionCorrect = correct
            };
            ServiceBusQueueMessageProducer.SendMessageToQueue(connectionString, ApplicationConstants.ResponseQueueName, response, message.SessionId);
        }
    }
}
