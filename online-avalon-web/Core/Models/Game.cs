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
        public int NumPlayers { get; set; }
        public bool Active { get; set; }
        public GameStatusEnum GameStatus { get; set; }
        public string UsernameWithLake { get; set; }
        public string KingUsername { get; set; }

        public List<Player> Players { get; set; }
    }
}
