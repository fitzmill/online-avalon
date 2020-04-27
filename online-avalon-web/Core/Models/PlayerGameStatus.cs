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
    } 
}
