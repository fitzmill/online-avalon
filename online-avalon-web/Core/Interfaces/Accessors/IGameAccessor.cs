using System;
using System.Linq;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Accessors
{
    public interface IGameAccessor
    {
        void MarkGameAsInactive(long gameId);
        /// <summary>
        /// Used to provide an "atomic" way of decrementing the player count
        /// since this number is volatile
        /// </summary>
        /// <param name="game"></param>
        void DecrementPlayerCount(Game game);
        /// <summary>
        /// Used to provide an "atomic" way of incrementing the player count
        /// since this number is volatile
        /// </summary>
        /// <param name="game"></param>
        void IncrementPlayerCount(Game game);
        /// <summary>
        /// Used to provide an "atomic" way to change the host username,
        /// since this field is volatile
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="hostUsername"></param>
        void SetHostUsername(long gameId, string hostUsername);
        void AddGame(Game game);
        Game GetGameWithPlayers(string publicGameId);
        Game GetGameWithPlayers(long gameId);
        Game GetGame(long gameId);
        Game GetGame(string publicGameId);
        IQueryable<Game> GetGames();
        void UpdateGame(Game game);
    }
}
