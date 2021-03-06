﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using online_avalon_web.Core.DTOs;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Interfaces.Engines;
using online_avalon_web.Core.Interfaces.Hubs;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Hubs
{
    public class GameHub : Hub<IGameHub>
    {
        private readonly IGameEngine _gameEngine;
        private readonly IPlayerEngine _playerEngine;
        public GameHub
            (
            IGameEngine gameEngine,
            IPlayerEngine playerEngine
            )
        {
            _gameEngine = gameEngine;
            _playerEngine = playerEngine;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, PublicGameId);
            await LeaveGame();
        }

        private long GameId
        {
            get
            {
                var item = Context.Items["GameId"];
                if (item == null)
                {
                    return -1;
                }

                return (long)item;
            }
            set
            {
                Context.Items["GameId"] = value;
            }
        }

        private string PublicGameId
        {
            get
            {
                return (string)Context.Items["PublicId"];
            }
            set
            {
                Context.Items["PublicId"] = value;
            }
        }

        private string Username
        {
            get
            {
                return (string)Context.Items["Username"];
            }
            set
            {
                Context.Items["Username"] = value;
            }
        }

        public async Task JoinGameAsHost(string publicGameId)
        {
            PublicGameId = publicGameId;
            var game = _gameEngine.GetGame(publicGameId);

            GameId = game.GameId;
            Username = game.HostUsername;

            await Groups.AddToGroupAsync(Context.ConnectionId, publicGameId);
        }

        public async Task<PlayerGameStatus> JoinGame(string publicGameId, string username)
        {

            PublicGameId = publicGameId;
            Username = username;

            var game = _gameEngine.AddPlayerToGame(username, publicGameId);

            await Clients.Group(publicGameId).ReceiveNewPlayer(game.Players.First((p) => p.Username == username));
            await Groups.AddToGroupAsync(Context.ConnectionId, publicGameId);

            GameId = game.GameId;

            return game;
        }

        public async Task LeaveGame()
        {
            if (GameId < 0) return;
            _gameEngine.RemovePlayerFromGame(GameId, Username, out string newHostUsername);
            await Clients.Group(PublicGameId).ReceiveDisconnectedPlayer(Username, newHostUsername);
        }

        public async Task StartGame(GameOptionsDTO gameOptions)
        {
            var info = _gameEngine.StartGame(PublicGameId, gameOptions.OptionalRoles);

            foreach(string username in info.Keys)
            {
                await Clients.User(CustomUserIdProvider.GetUserId(username, PublicGameId)).StartGame(info[username]);
            }
        }

        public async Task AddUserToParty(string username)
        {
            await Clients.Groups(PublicGameId).AddPlayerToParty(username);
            _playerEngine.AddPlayerToParty(GameId, username);
        }

        public async Task RemoveUserFromParty(string username)
        {
            await Clients.Groups(PublicGameId).RemovePlayerFromParty(username);
            _playerEngine.RemovePlayerFromParty(GameId, username);
        }

        public async Task SubmitParty()
        {
            if (!_gameEngine.TryMoveToApprovalStage(GameId, out IEnumerable<string> partyUsernames))
            {
                throw new InvalidOperationException("Invalid number of players have been added to the party");
            }

            await Clients.Groups(PublicGameId).StartApprovalVote(partyUsernames);
        }

        public async Task VoteForParty(ApprovalVoteOptionsEnum approvalVote)
        {
            _playerEngine.VoteForParty(GameId, Username, approvalVote);

            if (_gameEngine.TryToApproveParty(GameId, out Dictionary<string, ApprovalVoteOptionsEnum> userVotes, out string newKingUsername))
            {
                if (_gameEngine.HaveFivePartiesFailed(GameId))
                {
                    var gameSummary = _gameEngine.EndGame(GameId, GameResultEnum.EvilWins);
                    await Clients.Groups(PublicGameId).ReceiveUserApprovalVotes(userVotes, newKingUsername, gameSummary);
                }
                else
                {
                    await Clients.Groups(PublicGameId).ReceiveUserApprovalVotes(userVotes, newKingUsername);

                }
            }
        }

        public async Task VoteForQuest(QuestVoteOptionsEnum questVote)
        {
            _playerEngine.VoteForQuest(GameId, Username, questVote);

            if (_gameEngine.TryCompleteQuest(GameId, out List<QuestVoteOptionsEnum> questVotes))
            {
                await Clients.Group(PublicGameId).ReceiveQuestVotes(questVotes);
                await SendEndQuestInfo();
            }
        }

        public async Task SendEndQuestInfo()
        {
            if (_gameEngine.TryEvilWins(GameId, out Game gameSummary))
            {
                await Clients.Group(PublicGameId).EndGameAndReceiveSummary(gameSummary);
            }
            else if (_gameEngine.TryMoveToLakeStage(GameId, out string usernameWithLake, out IEnumerable<string> usernamesToLake))
            {
                await Clients.User(CustomUserIdProvider.GetUserId(usernameWithLake, PublicGameId)).ReceiveUsernamesToLake(usernamesToLake);
                await Clients.Group(PublicGameId).MoveToLakeStage(usernameWithLake);
            }
            else if (_gameEngine.TryMoveToNextQuest(GameId, out PlayerGameStatus updatedGame))
            {
                await Clients.Group(PublicGameId).ReceiveNewQuestInfo(new NewQuestDTO
                {
                    KingUsername = updatedGame.KingUsername,
                    UsernameWithLake = updatedGame.UsernameWithLake,
                    NewQuestNumber = updatedGame.QuestNumber,
                    RequiredNumPartyMembers = updatedGame.RequiredNumPartyMembers
                });
            }
            else if (_gameEngine.TryMoveToAssassinationStage(GameId, out string assassin, out IEnumerable<string> usernamesToAssassinate))
            {
                await Clients.User(CustomUserIdProvider.GetUserId(assassin, PublicGameId)).ReceiveUsernamesToAssassinate(usernamesToAssassinate);
                await Clients.Group(PublicGameId).MoveToAssassinationStage();
            }
            else
            {
                // this only happens if evil has succeeded in 3 quests
                var game = _gameEngine.EndGame(GameId, GameResultEnum.EvilWins);
                await Clients.Group(PublicGameId).EndGameAndReceiveSummary(game);
            }
        }

        public async Task LakePlayer(string username)
        {
            if (_playerEngine.TryLakePlayer(GameId, username, out AlignmentEnum? alignment))
            {
                await Clients.Caller.ReceiveLakeAlignment(alignment.Value);
                await Clients.Group(PublicGameId).ReceiveLakedUsername(username);
            }
            else
            {
                throw new ArgumentException($"{username} cannot be laked");
            }
        }

        public async Task ContinueEndQuestAfterLake()
        {
            if (_gameEngine.TryMoveToNextQuest(GameId, out PlayerGameStatus updatedGame))
            {
                await Clients.Group(PublicGameId).ReceiveNewQuestInfo(new NewQuestDTO
                {
                    KingUsername = updatedGame.KingUsername,
                    UsernameWithLake = updatedGame.UsernameWithLake,
                    NewQuestNumber = updatedGame.QuestNumber,
                    RequiredNumPartyMembers = updatedGame.RequiredNumPartyMembers
                });
            }
            else if (_gameEngine.TryMoveToAssassinationStage(GameId, out string assassin, out IEnumerable<string> usernamesToAssassinate))
            {
                await Clients.Group(PublicGameId).MoveToAssassinationStage();
                await Clients.User(CustomUserIdProvider.GetUserId(assassin, PublicGameId)).ReceiveUsernamesToAssassinate(usernamesToAssassinate);
            }
            else
            {
                // this only happens if evil has succeeded in 3 quests
                var game = _gameEngine.EndGame(GameId, GameResultEnum.EvilWins);
                await Clients.Group(PublicGameId).EndGameAndReceiveSummary(game);
            }
        }

        public async Task AssassinatePlayer(string username)
        {
            if (_playerEngine.TryAssassinatePlayer(GameId, username))
            {
                // Merlin was assassinated
                var game = _gameEngine.EndGame(GameId, GameResultEnum.EvilWins);
                await Clients.Group(PublicGameId).EndGameAndReceiveSummary(game);
            }
            else
            {
                var game = _gameEngine.EndGame(GameId, GameResultEnum.GoodWins);
                await Clients.Group(PublicGameId).EndGameAndReceiveSummary(game);
            }
        }

        public async Task RestartGame()
        {
            _gameEngine.RestartGame(GameId);

            await Clients.Group(PublicGameId).ResetGame();
        }

        public Task ResetConnection()
        {
            var game = _gameEngine.GetGame(PublicGameId);

            GameId = game.GameId;

            return Task.CompletedTask;
        }
    }
}
