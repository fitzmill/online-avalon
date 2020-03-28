using System;
using System.Collections.Generic;
using System.Linq;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Interfaces.Engines;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Engines
{
    public class GameEngine : IGameEngine
    {
        private readonly IGameAccessor _gameAccessor;
        private readonly IPlayerAccessor _playerAccessor;
        private readonly IQuestAccessor _questAccessor;
        public GameEngine(
            IGameAccessor gameAccessor,
            IPlayerAccessor playerAccessor,
            IQuestAccessor questAccessor
            )
        {
            _gameAccessor = gameAccessor;
            _playerAccessor = playerAccessor;
            _questAccessor = questAccessor;
        }

        public Game AddPlayerToGame(string username, string publicGameId)
        {
            var game = _gameAccessor.GetGameWithPlayers(publicGameId);
            game.Players.Add(new Player
            {
                Username = username
            });
            game.NumPlayers++;

            _gameAccessor.UpdateGame(game);
            return game;
        }

        public Game CreateGame(string hostUsername)
        {
            var game = new Game
            {
                Active = true,
                GameStatus = GameStatusEnum.PreGame,
                NumPlayers = 1,
                Players = new List<Player>(new[] { new Player
                {
                    Username = hostUsername
                } }),
                QuestNumber = 0
            };

            _gameAccessor.AddGame(game);

            return game;
        }

        public void RemovePlayerFromGame(long gameId, string username)
        {
            var game = _gameAccessor.GetGame(gameId);

            var player = game.Players.FirstOrDefault(p => p.Username == username);

            if (player == default(Player))
            {
                return;
            }

            _playerAccessor.RemovePlayer(player);
            game.NumPlayers--;
            _gameAccessor.UpdateGame(game);
        }

        public Dictionary<string, PlayerGameStatus> StartGame(string publicGameId, IEnumerable<RoleEnum> optionalRoles)
        {
            if (string.IsNullOrEmpty(publicGameId)) throw new ArgumentNullException($"Null argument {nameof(publicGameId)}");
            if (optionalRoles == null) throw new ArgumentNullException($"Null argument {nameof(optionalRoles)}");

            var game = _gameAccessor.GetGameWithPlayers(publicGameId);

            if (game.NumPlayers < Utilities.MinNumberOfPlayers || game.NumPlayers > Utilities.MaxNumberOfPlayers)
            {
                throw new InvalidOperationException($"Invalid number of players for game {publicGameId}");
            }

            AssignPlayerRoles(optionalRoles, game);

            var kingIndex = Utilities.random.Next(game.NumPlayers);
            var lakeIndex = (kingIndex + 1) % game.NumPlayers;

            game.KingUsername = game.Players[kingIndex].Username;
            game.UsernameWithLake = game.Players[lakeIndex].Username;
            game.QuestNumber = 1;

            _gameAccessor.UpdateGame(game);
            _questAccessor.AddQuest(new Quest
            {
                QuestNumber = game.QuestNumber,
                GameId = game.GameId
            });

            return game.Players
                .ToDictionary(p => p.Username, p => new PlayerGameStatus
                {
                    GameId = game.GameId,
                    King = game.KingUsername,
                    PlayerRole = p.Role,
                    UsernameWithLake = game.UsernameWithLake,
                    KnownUsernames = Utilities.GetKnownUsernamesForPlayer(p, game.Players)
                });
        }

        private void AssignPlayerRoles(IEnumerable<RoleEnum> optionalRoles, Game game)
        {
            if (game.NumPlayers < optionalRoles.Count() + 2)
            {
                throw new ArgumentException("Not enough players to fulfill every role");
            }

            var shuffledPlayerIndices = Utilities.GetRandomSequence(game.NumPlayers);

            // assign roles to players starting with required roles
            game.Players[shuffledPlayerIndices[0]].Role = RoleEnum.Merlin;
            game.Players[shuffledPlayerIndices[1]].Role = RoleEnum.Assassin;

            var numGoodPlayers = 1;
            var numBadPlayers = 1;

            // next fill optional roles (Percival, Morgana, etc)
            foreach (RoleEnum role in optionalRoles)
            {
                game.Players[shuffledPlayerIndices[numGoodPlayers + numBadPlayers]].Role = role;

                var alignment = Utilities.GetAlignmentForRole(role);
                if (alignment == AlignmentEnum.Good) numGoodPlayers++;
                if (alignment == AlignmentEnum.Evil) numBadPlayers++;
            }

            var tuple = Utilities.GetNumGoodBadPlayersInGame(game.NumPlayers);

            // place rest of players as either loyal servants or minions of mordred
            while (numGoodPlayers < tuple.Item1)
            {
                game.Players[shuffledPlayerIndices[numGoodPlayers + numBadPlayers]].Role = RoleEnum.LoyalServantOfArthur;
                numGoodPlayers++;
            }
            while (numBadPlayers < tuple.Item2)
            {
                game.Players[shuffledPlayerIndices[numGoodPlayers + numBadPlayers]].Role = RoleEnum.MinionOfMordred;
                numBadPlayers++;
            }
        }

        public bool TryCompleteQuest(long gameId, out List<QuestVoteOptionsEnum> questVotes)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            var requiredNumVotes = Utilities.GetRequiredQuestVotes(game.QuestNumber, game.NumPlayers);
            questVotes = game.Players
                .Where(p => p.QuestVote.HasValue)
                .Select(p => p.QuestVote.Value)
                .ToList();

            if (questVotes.Count < requiredNumVotes)
            {
                questVotes = null;
                return false;
            }

            questVotes = Utilities.ShuffleList(questVotes);

            var quest = game.Quests.First(q => q.QuestNumber == game.QuestNumber);
            quest.QuestResult = questVotes.Any(v => v == QuestVoteOptionsEnum.Fail) ? QuestResultEnum.EvilWins : QuestResultEnum.GoodWins;
            _questAccessor.UpdateQuest(quest);

            return true;
        }

        public bool TryToApproveParty(long gameId, out Dictionary<string, ApprovalVoteOptionsEnum> userVotes)
        {
            var players = _playerAccessor.GetPlayersInGame(gameId).ToList();

            if (players.Count(p => p.ApprovalVote.HasValue) < players.Count)
            {
                userVotes = null;
                return false;
            }

            userVotes = players.ToDictionary(p => p.Username, p => p.ApprovalVote.Value);
            return true;
        }

        public bool TryMoveToLakeStage(long gameId, out string usernameWithLake)
        {
            var game = _gameAccessor.GetGame(gameId);

            switch (game.QuestNumber)
            {
                case 2:
                case 3:
                case 4:
                    usernameWithLake = game.UsernameWithLake;
                    return true;
                default:
                    usernameWithLake = null;
                    return false;
            }
        }

        public bool TryMoveToNextQuest(long gameId, out int newQuestNumber)
        {
            var game = _gameAccessor.GetGame(gameId);

            if (game.QuestNumber < 5
                && game.Quests.Count(q => q.QuestResult == QuestResultEnum.GoodWins) < 3
                && game.Quests.Count(q => q.QuestResult == QuestResultEnum.EvilWins) < 3)
            {
                game.QuestNumber++;
                _questAccessor.AddQuest(new Quest
                {
                    QuestNumber = game.QuestNumber,
                    GameId = game.GameId
                });
                _gameAccessor.UpdateGame(game);
                newQuestNumber = game.QuestNumber;
                return true;
            }
            else
            {
                newQuestNumber = 0;
                return false;
            }
        }

        public bool TryMoveToAssassinationStage(long gameId, out IEnumerable<string> usernamesToAssassinate)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            if (game.Quests.Count(q => q.QuestResult == QuestResultEnum.GoodWins) == 3)
            {
                usernamesToAssassinate = game.Players
                    .Where(p => Utilities.GetAlignmentForRole(p.Role) == AlignmentEnum.Good)
                    .Select(p => p.Username);
                return true;
            }
            else
            {
                usernamesToAssassinate = null;
                return false;
            }
        }

        public bool TryMoveToApprovalStage(long gameId)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            var numRequiredPartyMembers = Utilities.GetRequiredQuestVotes(game.QuestNumber, game.NumPlayers);

            return game.Players.Count(p => p.InParty) == numRequiredPartyMembers;
        }

        public Game EndGame(long gameId)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            game.GameStatus = GameStatusEnum.Finished;

            _gameAccessor.UpdateGame(game);

            return game;
        }
    }
}
