using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using online_avalon_web.Core;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Accessors
{
    public class GameAccessor : IGameAccessor
    {
        private readonly AvalonContext _avalonContext;
        public GameAccessor(AvalonContext avalonContext)
        {
            _avalonContext = avalonContext;
        }

        public void AddGame(Game game)
        {
            _avalonContext.Games.Add(game);
            _avalonContext.SaveChanges();
        }

        public Game GetGame(long gameId)
        {
            return _avalonContext.Games
                .Include(g => g.Quests)
                .FirstOrDefault(g => g.GameId == gameId);
        }

        public Game GetGameWithPlayers(string publicGameId)
        {
            return _avalonContext.Games
                .Include(g => g.Quests)
                .Include(g => g.Players)
                .FirstOrDefault(g => g.Active && g.PublicId == publicGameId);
        }

        public Game GetGameWithPlayers(long gameId)
        {
            return _avalonContext.Games
                .Include(g => g.Quests)
                .Include(g => g.Players)
                .FirstOrDefault(g => g.Active && g.GameId == gameId);
        }

        public void MarkGameAsInactive(long gameId)
        {
            var game = _avalonContext.Games.First(g => g.Active && g.GameId == gameId);
            game.Active = false;
            _avalonContext.Update(game);
            _avalonContext.SaveChanges();
        }

        public void UpdateGame(Game game)
        {
            _avalonContext.Games.Update(game);
            _avalonContext.SaveChanges();
        }
    }
}
