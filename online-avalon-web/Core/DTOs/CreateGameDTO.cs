using System;
using System.ComponentModel.DataAnnotations;

namespace online_avalon_web.Core.DTOs
{
    public class CreateGameDTO
    {
        public CreateGameDTO()
        {
        }

        [Required]
        public string HostUsername { get; set; }
        [Required]
        public string PublicGameId { get; set; }
    }
}
