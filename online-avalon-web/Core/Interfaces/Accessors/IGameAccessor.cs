using System;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Accessors
{
    public interface IGameAccessor
    {
        void MarkGameAsInactive(string publicGameId);
        void AddGame(Game game);
        Game GetGameWithPlayers(string publicGameId);
    }
}
