using System;
using System.Collections.Generic;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.Models
{
    public class Game
    {
        public Game()
        {
        }

        public long GameId { get; set; }
        public string PublicId { get; set; }
        public string HostUsername { get; set; }
        public int NumPlayers { get; set; }
        public bool Active { get; set; }
        public GameStatusEnum GameStatus { get; set; }
        public string UsernameWithLake { get; set; }
        public string KingUsername { get; set; }
        public int QuestNumber { get; set; }
        public int PartyNumber { get; set; }
        public GameResultEnum? GameResult { get; set; }

        public List<Player> Players { get; set; }
        public List<Quest> Quests { get; set; }
    }
}
