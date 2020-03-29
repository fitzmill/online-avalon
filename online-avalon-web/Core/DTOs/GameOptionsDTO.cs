using System;
using System.Collections.Generic;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.DTOs
{
    public class GameOptionsDTO
    {
        public GameOptionsDTO()
        {
        }

        public IEnumerable<RoleEnum> OptionalRoles { get; set; }
    }
}
