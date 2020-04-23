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

            if (game.GameStatus != GameStatusEnum.PreGame)
            {
                throw new InvalidOperationException("Game has already started");
            }
            else if (game.Players.Any(p => p.Username == username))
            {
                throw new ArgumentException($"There is already a user in the game with the name {username}");
            }

            // Check if player is reconnecting
            var disconnectedPlayer = _playerAccessor.GetPlayer(game.GameId, username);
            if (disconnectedPlayer != default(Player))
            {
                disconnectedPlayer.Disconnected = false;
            }
            else
            {
                game.Players.Add(new Player
                {
                    Username = username
                });
            }

            game.NumPlayers++;

            _gameAccessor.UpdateGame(game);
            return new Game
            {
                GameId = game.GameId,
                GameResult = game.GameResult,
                Active = game.Active,
                GameStatus = game.GameStatus,
                HostUsername = game.HostUsername,
                KingUsername = game.KingUsername,
                NumPlayers = game.NumPlayers,
                PartyNumber = game.PartyNumber,
                PublicId = game.PublicId,
                QuestNumber = game.QuestNumber,
                Quests = game.Quests,
                UsernameWithLake = game.UsernameWithLake,
                Players = game.Players.Select(p => new Player
                {
                    Username = p.Username,
                    ApprovalVote = p.ApprovalVote,
                    InParty = p.InParty
                }).ToList()
            };
        }

        public bool TryCreateGame(string hostUsername, string publicGameId, out Game game)
        {
            game = _gameAccessor.GetGameWithPlayers(publicGameId);

            if (game == default(Game) || game.Active == false)
            {
                game = new Game
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

                return true;
            }

            game = null;
            return false;
        }

        public void RemovePlayerFromGame(long gameId, string username)
        {
            var game = _gameAccessor.GetGame(gameId);

            var player = game.Players.FirstOrDefault(p => p.Username == username);

            if (player == default(Player))
            {
                return;
            }

            game.NumPlayers--;

            if (player.Username == game.HostUsername)
            {
                if (game.NumPlayers == 0)
                {
                    DeactivateGame(game.PublicId);
                    return;
                }
                else
                {
                    game.HostUsername = game.Players.First(p => p.Username != game.HostUsername).Username;
                }
            }

            player.Disconnected = true;

            _playerAccessor.UpdatePlayer(player);
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
            game.Players[lakeIndex].HasHeldLake = true;
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

        public bool TryToApproveParty(long gameId, out Dictionary<string, ApprovalVoteOptionsEnum> userVotes, out string newKingUsername)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);
            var players = game.Players;

            if (players.Count(p => p.ApprovalVote.HasValue) < players.Count)
            {
                userVotes = null;
                newKingUsername = null;
                return false;
            }

            userVotes = players.ToDictionary(p => p.Username, p => p.ApprovalVote.Value);
            newKingUsername = null;
            //check if party fails
            if (players.Count(p => p.ApprovalVote == ApprovalVoteOptionsEnum.Approve) < players.Count / 2)
            {
                // assign new king
                var newKingIndex = (players.FindIndex(p => p.Username == game.KingUsername) + 1) % game.NumPlayers;
                newKingUsername = players[newKingIndex].Username;
                game.KingUsername = newKingUsername;
                // update party number
                game.PartyNumber++;
                // reset players
                foreach (Player p in players)
                {
                    p.ApprovalVote = null;
                }
                _gameAccessor.UpdateGame(game);
            }
            return true;
        }

        public bool TryMoveToLakeStage(long gameId, out string usernameWithLake, out IEnumerable<string> usernamesToLake)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            switch (game.QuestNumber)
            {
                case 2:
                case 3:
                case 4:
                    usernamesToLake = game.Players.Where(p => !p.HasHeldLake).Select(p => p.Username);
                    usernameWithLake = game.UsernameWithLake;
                    return true;
                default:
                    usernameWithLake = null;
                    usernamesToLake = null;
                    return false;
            }
        }

        public bool TryMoveToNextQuest(long gameId, out Game updatedGame)
        {
            var game = _gameAccessor.GetGame(gameId);

            if (game.QuestNumber < 5
                && game.Quests.Count(q => q.QuestResult == QuestResultEnum.GoodWins) < 3
                && game.Quests.Count(q => q.QuestResult == QuestResultEnum.EvilWins) < 3)
            {
                // update quest
                game.QuestNumber++;
                _questAccessor.AddQuest(new Quest
                {
                    QuestNumber = game.QuestNumber,
                    GameId = game.GameId
                });

                // reset players
                foreach(Player player in game.Players)
                {
                    player.ApprovalVote = null;
                    player.QuestVote = null;
                }

                // update game
                var newKingIndex = (game.Players.FindIndex(p => p.Username == game.KingUsername) + 1) % game.NumPlayers;
                game.KingUsername = game.Players[newKingIndex].Username;

                game.PartyNumber = 1;

                _gameAccessor.UpdateGame(game);
                updatedGame = game;
                return true;
            }
            else
            {
                updatedGame = null;
                return false;
            }
        }

        public bool TryMoveToAssassinationStage(long gameId, out string assassin, out IEnumerable<string> usernamesToAssassinate)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            if (game.Quests.Count(q => q.QuestResult == QuestResultEnum.GoodWins) == 3)
            {
                usernamesToAssassinate = game.Players
                    .Where(p => Utilities.GetAlignmentForRole(p.Role) == AlignmentEnum.Good)
                    .Select(p => p.Username);
                assassin = game.Players.First(p => p.Role == RoleEnum.Assassin).Username;
                return true;
            }
            else
            {
                usernamesToAssassinate = null;
                assassin = null;
                return false;
            }
        }

        public bool TryMoveToApprovalStage(long gameId, out IEnumerable<string> partyUsernames)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            var numRequiredPartyMembers = Utilities.GetRequiredQuestVotes(game.QuestNumber, game.NumPlayers);

            partyUsernames = game.Players.Where(p => p.InParty).Select(p => p.Username);

            return partyUsernames.Count() == numRequiredPartyMembers;
        }

        public Game EndGame(long gameId, GameResultEnum gameResult)
        {
            var game = _gameAccessor.GetGame(gameId);

            game.GameStatus = GameStatusEnum.Finished;
            game.GameResult = gameResult;

            _gameAccessor.UpdateGame(game);

            return game;
        }

        public Game RestartGame(long gameId)
        {
            var oldGame = _gameAccessor.GetGameWithPlayers(gameId);

            oldGame.Active = false;

            _gameAccessor.UpdateGame(oldGame);

            var newGame = new Game
            {
                Active = true,
                GameStatus = GameStatusEnum.PreGame,
                HostUsername = oldGame.HostUsername,
                NumPlayers = oldGame.NumPlayers,
                PublicId = oldGame.PublicId,
                QuestNumber = 0,
                Players = oldGame.Players.Select(p => new Player
                {
                    Username = p.Username
                }).ToList()
            };

            _gameAccessor.AddGame(newGame);

            return newGame;
        }

        public void DeactivateGame(string publicGameId)
        {
            var game = _gameAccessor.GetGame(publicGameId);

            game.Active = false;

            _gameAccessor.UpdateGame(game);
        }

        public Game GetGame(string publicGameId)
        {
            return _gameAccessor.GetGameWithPlayers(publicGameId);
        }

        public bool TryEvilWins(long gameId, out Game gameSummary)
        {
            var game = _gameAccessor.GetGameWithPlayers(gameId);

            if (game.Quests.Count(q => q.QuestResult == QuestResultEnum.EvilWins) == 3)
            {
                gameSummary = game;
                EndGame(gameId, GameResultEnum.EvilWins);
                return true;
            }
            else
            {
                gameSummary = null;
                return false;
            }
        }

        public bool HaveFivePartiesFailed(long gameId)
        {
            var game = _gameAccessor.GetGame(gameId);

            return game.PartyNumber == 6;
        }
    }
}
