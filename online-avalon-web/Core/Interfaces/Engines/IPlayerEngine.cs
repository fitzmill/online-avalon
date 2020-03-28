using System;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.Interfaces.Engines
{
    public interface IPlayerEngine
    {
        void AddPlayerToParty(long gameId, string username);
        void RemovePlayerFromParty(long gameId, string username);
        void VoteForParty(long gameId, string username, ApprovalVoteOptionsEnum approvalVote);
        void VoteForQuest(long gameId, string username, QuestVoteOptionsEnum questVote);
        bool TryLakePlayer(long gameId, string username, out AlignmentEnum? alignment);
    }
}
