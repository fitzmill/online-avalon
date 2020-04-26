using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using online_avalon_web.Core.DTOs;
using online_avalon_web.Core.Interfaces.Engines;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameEngine _gameEngine;

        public GameController(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        [HttpPost]
        public IActionResult CreateGame([FromBody] CreateGameDTO createGameDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid parameters");
            }

            if (_gameEngine.TryCreateGame(createGameDTO.HostUsername, createGameDTO.PublicGameId, out Game game))
            {
                return Ok(game);
            }
            else
            {
                return Conflict("Game already active with the same id");
            }
        }
    }
}