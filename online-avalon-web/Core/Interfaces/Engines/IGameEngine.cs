using System;
using System.Collections.Generic;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Engines
{
    public interface IGameEngine
    {
        Game CreateGame(string hostUsername);
        Game AddPlayerToGame(string username, string publicGameId);
        void RemovePlayerFromGame(long gameId, string username);
        Dictionary<string, PlayerGameStatus> StartGame(string publicGameId, IEnumerable<RoleEnum> optionalRoles);
        bool TryMoveToApprovalStage(long gameId);
        bool TryToApproveParty(long gameId, out Dictionary<string, ApprovalVoteOptionsEnum> userVotes);
        bool TryCompleteQuest(long gameId, out List<QuestVoteOptionsEnum> questVotes);
        bool TryMoveToLakeStage(long gameId, out string usernameWithLake);
        bool TryMoveToNextQuest(long gameId, out int newQuestNumber);
        bool TryMoveToAssassinationStage(long gameId, out IEnumerable<string> usernamesToAssassinate);
        Game EndGame(long gameId);
    }
}
