using System;
using System.Linq;
using online_avalon_web.Core;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Accessors
{
    public class PlayerAccessor : IPlayerAccessor
    {
        private readonly AvalonContext _avalonContext;
        public PlayerAccessor(AvalonContext avalonContext)
        {
            _avalonContext = avalonContext;
        }

        public void AddPlayer(Player player)
        {
            _avalonContext.Players.Add(player);
            _avalonContext.SaveChanges();
        }

        public Player GetPlayer(long gameId, string username)
        {
            return _avalonContext.Players.FirstOrDefault(p => p.GameId == gameId && p.Username == username);
        }

        public IQueryable<Player> GetPlayersInGame(long gameId)
        {
            return _avalonContext.Players.Where(p => p.GameId == gameId);
        }

        public void RemovePlayer(Player player)
        {
            _avalonContext.Remove(player);
            _avalonContext.SaveChanges();
        }

        public void UpdatePlayer(Player player)
        {
            _avalonContext.Update(player);
            _avalonContext.SaveChanges();
        }
    }
}
