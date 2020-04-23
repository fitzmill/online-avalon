using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using online_avalon_web.Core.DTOs;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Hubs
{
    public interface IGameHub
    {
        Task ReceiveNewPlayer(string username, string message);
        Task StartGame(PlayerGameStatus status);
        Task AddPlayerToParty(string username);
        Task RemovePlayerFromParty(string username);
        Task StartApprovalVote(IEnumerable<string> partyUsernames);
        Task ReceiveUserApprovalVotes(Dictionary<string, ApprovalVoteOptionsEnum> userVotes, string newKingUsername);
        Task ReceiveUserApprovalVotes(Dictionary<string, ApprovalVoteOptionsEnum> userVotes, string newKingUsername, Game gameSummary);
        Task ReceiveQuestVotes(IEnumerable<QuestVoteOptionsEnum> questVotes);
        Task MoveToLakeStage(string usernameWithLake);
        Task ReceiveUsernamesToLake(IEnumerable<string> usernamesToLake);
        Task ReceiveLakedUsername(string username);
        Task ReceiveNewQuestInfo(NewQuestDTO newQuestInfo);
        Task MoveToAssassinationStage();
        Task ReceiveUsernamesToAssassinate(IEnumerable<string> usernamesToAssassinate);
        Task EndGameAndReceiveSummary(Game game);
        Task ReceiveLakeAlignment(AlignmentEnum alignment);
        Task ResetGame();
    }
}
