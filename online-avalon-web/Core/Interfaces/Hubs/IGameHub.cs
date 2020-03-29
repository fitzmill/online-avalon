using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Hubs
{
    public interface IGameHub
    {
        Task ReceiveNewPlayer(string message);
        Task StartGame(PlayerGameStatus status);
        Task AddPlayerToParty(string username);
        Task RemovePlayerFromParty(string username);
        Task StartApprovalVote(IEnumerable<string> partyUsernames);
        Task ReceiveUserApprovalVotes(Dictionary<string, ApprovalVoteOptionsEnum> userVotes);
        Task ReceiveQuestVotes(IEnumerable<QuestVoteOptionsEnum> questVotes);
        Task MoveToLakeStage(string usernameWithLake);
        Task ReceiveUsernamesToLake(IEnumerable<string> usernamesToLake);
        Task ReceiveNewQuestNumber(int questNumber);
        Task MoveToAssassinationStage();
        Task ReceiveUsernamesToAssassinate(IEnumerable<string> usernamesToAssassinate);
        Task EndGameAndReceiveSummary(Game game);
    }
}
