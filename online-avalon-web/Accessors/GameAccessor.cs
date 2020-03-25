using System;
using System.Linq;
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

        public Game GetGameWithPlayers(string publicGameId)
        {
            return _avalonContext.Games.First(g => g.Active && g.PublicId == publicGameId);
        }

        public void MarkGameAsInactive(string publicGameId)
        {
            var game = _avalonContext.Games.First(g => g.Active && g.PublicId == publicGameId);
            game.Active = false;
            _avalonContext.Update(game);
            _avalonContext.SaveChanges();
        }
    }
}
