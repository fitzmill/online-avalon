using System;
using System.Collections.Generic;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.Models
{
    public class PlayerGameStatus
    {
        public PlayerGameStatus()
        {
        }

        public RoleEnum PlayerRole { get; set; }
        public long GameId { get; set; }
        public IEnumerable<string> KnownUsernames { get; set; }
        public string UsernameWithLake { get; set; }
        public string KingUsername { get; set; }
        public string HostUsername { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public int NumPlayers { get; set; }
        public QuestStageEnum QuestStage { get; set; }
        public int QuestNumber { get; set; }
        public int RequiredNumPartyMembers { get; set; }
        public int PartyNumber { get; set; }
        public IEnumerable<Quest> Quests { get; set; }
    } 
}
