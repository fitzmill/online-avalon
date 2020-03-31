using System;
using System.Collections.Generic;
using System.Linq;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Accessors
{
    public interface IPlayerAccessor
    {
        void AddPlayer(Player player);
        void RemovePlayer(Player player);
        Player GetPlayer(long gameId, string username);
        IQueryable<Player> GetPlayersInGame(long gameId);
        void UpdatePlayer(Player player);
        void UpdatePlayers(IEnumerable<Player> players);
    }
}
