using System;
using System.Collections.Generic;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Engines
{
    public interface IGameEngine
    {
        Game CreateGame(string hostUsername);
        Game AddPlayerToGame(string username, string publicGameId);
        Dictionary<string, PlayerGameStatus> StartGame(string publicGameId);
        bool CanMoveToApprovalStage(long gameId);
    }
}
