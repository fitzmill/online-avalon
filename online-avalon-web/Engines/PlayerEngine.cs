using System;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Interfaces.Engines;

namespace online_avalon_web.Engines
{
    public class PlayerEngine : IPlayerEngine
    {
        private readonly IPlayerAccessor _playerAccessor;
        public PlayerEngine(IPlayerAccessor playerAccessor)
        {
            _playerAccessor = playerAccessor;
        }

        public void AddPlayerToParty(long gameId, string username)
        {
            var player = _playerAccessor.GetPlayer(gameId, username);

            player.InParty = true;

            _playerAccessor.UpdatePlayer(player);
        }

        public void RemovePlayerFromParty(long gameId, string username)
        {
            var player = _playerAccessor.GetPlayer(gameId, username);

            player.InParty = false;

            _playerAccessor.UpdatePlayer(player);
        }

        public bool TryAssassinatePlayer(long gameId, string username)
        {
            var player = _playerAccessor.GetPlayer(gameId, username);

            return player.Role == RoleEnum.Merlin;
        }

        public bool TryLakePlayer(long gameId, string username, out AlignmentEnum? alignment)
        {
            var player = _playerAccessor.GetPlayer(gameId, username);

            if (player.HasHeldLake)
            {
                alignment = null;
                return false;
            }

            alignment = Utilities.GetAlignmentForRole(player.Role);

            return true;
        }

        public void VoteForParty(long gameId, string username, ApprovalVoteOptionsEnum approvalVote)
        {
            var player = _playerAccessor.GetPlayer(gameId, username);

            player.ApprovalVote = approvalVote;

            _playerAccessor.UpdatePlayer(player);
        }

        public void VoteForQuest(long gameId, string username, QuestVoteOptionsEnum questVote)
        {
            var player = _playerAccessor.GetPlayer(gameId, username);

            player.QuestVote = questVote;

            _playerAccessor.UpdatePlayer(player);
        }
    }
}
