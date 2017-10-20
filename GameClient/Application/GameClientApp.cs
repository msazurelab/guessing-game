using System;
using Microsoft.ServiceBus.Messaging;
using Interface;
using Newtonsoft.Json;
using Interface.Requests;
using Utils;
using Interface.Responses;
using Interface.Model;
using System.Configuration;

namespace GameClient.Application
{
    public class GameClientApp
    {
        const int maxGuessAttempts = 5;
        private string _connectionString;
        private bool _isGameOver;
        private int _guessAttempts;
        private Question[] _questions;

        public GameClientApp()
        {
            _connectionString = ConfigurationSettings.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Press enter to start a new game");
                Console.ReadLine();

                _isGameOver = false;
                _guessAttempts = 0;

                var sessionId = Guid.NewGuid().ToString();
                var startGameRequest = new StartGameRequest
                {
                    SessionId = sessionId
                };
                ServiceBusQueueMessageProducer.SendMessageToQueue(_connectionString, ApplicationConstants.RequestQueueName, startGameRequest);
                PollAndHandleMessage(sessionId);

                while (true)
                {
                    Console.WriteLine("Please choose an option below:");
                    Console.WriteLine("1. Ask a question");
                    Console.WriteLine("2. Guess Answer");
                    Console.WriteLine();

                    var choice = Console.ReadLine();
                    Console.WriteLine();

                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Please enter question id:");
                            Utils.DisplayQuestions(_questions);
                            Console.WriteLine();

                            var questionString = Console.ReadLine();
                            int questionId;
                            if (!int.TryParse(questionString, out questionId))
                            {
                                Console.WriteLine("Invalid question Id. Please try again.");
                                continue;
                            }

                            var askQuestionRequest = new AskQuestionRequest(questionId);
                            ServiceBusQueueMessageProducer.SendMessageToQueue(_connectionString, ApplicationConstants.RequestQueueName, askQuestionRequest, sessionId);
                            PollAndHandleMessage(sessionId);
                            break;

                        case "2":
                            Console.WriteLine("Please enter your guess");
                            var guess = Console.ReadLine();
                            Console.WriteLine();

                            var guessAnswerRequest = new GuessAnswerRequest(guess);
                            ServiceBusQueueMessageProducer.SendMessageToQueue(_connectionString, ApplicationConstants.RequestQueueName, guessAnswerRequest, sessionId);
                            PollAndHandleMessage(sessionId);
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            Console.WriteLine();
                            break;
                    }

                    if (_isGameOver)
                    {
                        Console.WriteLine("Game ended. Press any key to continue.");
                        Console.ReadLine();
                        break;
                    }
                }
            }
        }


        public void PollAndHandleMessage(string sessionId)
        {
            while (true)
            {
                var queueClient = QueueClient.CreateFromConnectionString(_connectionString, ApplicationConstants.ResponseQueueName, ReceiveMode.ReceiveAndDelete);
                var session = queueClient.AcceptMessageSession(sessionId);

                var message = session.Receive();
                if (message == null)
                {
                    continue;
                }
                try
                {
                    string messageBody = message.GetBody<string>();
                    var response = JsonConvert.DeserializeObject<Response>(messageBody);
                    switch (response.Type)
                    {
                        case ResponseType.StartGame:
                            HandleStartGameResponse(messageBody);
                            break;

                        case ResponseType.AskQuestion:
                            HandleAskQuestionResponse(messageBody);
                            break;

                        case ResponseType.GuessAnswer:
                            HandleGuessAnswerResponse(messageBody);
                            break;
                    }
                    return;
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
                finally
                {
                    session.Close();
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void HandleStartGameResponse(string messageBody)
        {
            var startGameResponse = JsonConvert.DeserializeObject<StartGameResponse>(messageBody);
            _questions = startGameResponse.Questions;
            Console.WriteLine(startGameResponse.Message);
            Console.WriteLine();
        }

        private static void HandleAskQuestionResponse(string messageBody)
        {
            var askQuestionResponse = JsonConvert.DeserializeObject<AskQuestionResponse>(messageBody);
            string answer = askQuestionResponse.IsTrue ? "Yes" : "No";
            Console.WriteLine($"Answer: {answer}");
            Console.WriteLine();
        }

        private void HandleGuessAnswerResponse(string messageBody)
        {
            var guessAnswerResponse = JsonConvert.DeserializeObject<GuessAnswerResponse>(messageBody);
            if (guessAnswerResponse.IsSolutionCorrect)
            {
                Console.WriteLine("Congratulation! You've won!");
                _isGameOver = true;
            }
            else
            {
                if (++_guessAttempts >= maxGuessAttempts)
                {
                    Console.WriteLine("Oops, you didn't get it right! Better luck next time! :)");
                    _isGameOver = true;
                }
                else
                {
                    Console.WriteLine($"You have {maxGuessAttempts - _guessAttempts} guess attempts left");
                }
            }
            Console.WriteLine();
        }
    }
}
