using GameServer.Model;
using Interface.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Repository;

namespace GameServer.Application
{
    public class GameSessionManager
    {
        private IRepository<GameSession> _gameSessionRepository { get; set; }
        private Game[] _games;
        private Question[] _questions;

        private static readonly GameSessionManager _instance = new GameSessionManager();

        private GameSessionManager()
        {
            _gameSessionRepository = new InMemRepository<GameSession>();

            var gameDataJson = System.IO.File.ReadAllText(@".\Data\GameData.json");
            _games = JsonConvert.DeserializeObject<Game[]>(gameDataJson);

            var questionDataJson = System.IO.File.ReadAllText(@".\Data\QuestionData.json");
            _questions = JsonConvert.DeserializeObject<Question[]>(questionDataJson);
        }

        public static GameSessionManager Instance
        {
            get
            {
                return _instance;
            }
        }
        
        public Question[] GetQuestions()
        {
            return _questions;
        }

        public GameSession StartGameSession(string sessionId)
        {
            // Pick a random game, and create a session
            int randomNumber = new Random().Next(0, _games.Length);
            var chosenGame = _games[randomNumber];
            var gameSession = new GameSession(sessionId, chosenGame);

            _gameSessionRepository.Add(gameSession);
            return gameSession;
        }

        public bool AnswerQuestion(string sessionId, int questionId)
        {
            ValidateGameSession(sessionId);
            var session = _gameSessionRepository.GetById(sessionId);
            return session.Game.Hints.First(x => x.QuestionId == questionId).Answer;
        }

        public bool GuessAnswer(string sessionId, string guess)
        {
            ValidateGameSession(sessionId);
            var session = _gameSessionRepository.GetById(sessionId);
            return string.Compare(guess.Trim(), session.Game.Solution, true) == 0;
        }

        private void ValidateGameSession(string sessionId)
        {
            if (!_gameSessionRepository.Exists(sessionId))
            {
                throw new Exception($"Game session {sessionId} does not exist");
            }
        }


    }
}
