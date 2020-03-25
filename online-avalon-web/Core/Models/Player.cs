using System;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.Models
{
    public class Player
    {
        public Player()
        {
        }

        public long PlayerId { get; set; }
        public string Username { get; set; }
        public long GameId { get; set; }
        public RoleEnum Role { get; set; }
        public ApprovalVoteOptionsEnum ApprovalVote { get; set; }
        public QuestVoteOptionsEnum QuestVote { get; set; }

        public Game Game { get; set; }
    }
}
