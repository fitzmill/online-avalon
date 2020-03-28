using System;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Accessors
{
    public interface IGameAccessor
    {
        void MarkGameAsInactive(long gameId);
        void AddGame(Game game);
        Game GetGameWithPlayers(string publicGameId);
        Game GetGameWithPlayers(long gameId);
        Game GetGame(long gameId);
        Game GetGame(string publicGameId);
        void UpdateGame(Game game);
    }
}
