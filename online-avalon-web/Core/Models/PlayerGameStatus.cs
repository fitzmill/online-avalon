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
        public Dictionary<string, RoleEnum> KnownRoles { get; set; }
        public string UsernameWithLake { get; set; }
        public string King { get; set; }
    } 
}
