using System;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.Interfaces.Engines
{
    public interface IPlayerEngine
    {
        /// <summary>
        /// Updates player to be in the party
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="username"></param>
        void AddPlayerToParty(long gameId, string username);

        /// <summary>
        /// Updates player to be removed from the party
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="username"></param>
        void RemovePlayerFromParty(long gameId, string username);

        /// <summary>
        /// Records whether a user approves of a party
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="username"></param>
        /// <param name="approvalVote"></param>
        void VoteForParty(long gameId, string username, ApprovalVoteOptionsEnum approvalVote);

        /// <summary>
        /// Records the user's vote to succeed or fail a quest
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="username"></param>
        /// <param name="questVote"></param>
        void VoteForQuest(long gameId, string username, QuestVoteOptionsEnum questVote);

        /// <summary>
        /// Tries to lake a player. If the player has held the lake, then the function
        /// will return false and alignment will be null. Otherwise, the function will
        /// return true and alignment will be the alignment for the username
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="username"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        bool TryLakePlayer(long gameId, string username, out AlignmentEnum? alignment);
    }
}
