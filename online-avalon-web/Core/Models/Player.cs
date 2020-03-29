using System;
using System.Text.Json.Serialization;
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
        public ApprovalVoteOptionsEnum? ApprovalVote { get; set; }
        public QuestVoteOptionsEnum? QuestVote { get; set; }
        public bool InParty { get; set; }
        public bool HasHeldLake { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }
    }
}
