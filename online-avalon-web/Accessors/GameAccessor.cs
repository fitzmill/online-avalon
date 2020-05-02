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

        public void DecrementPlayerCount(Game game)
        {
            using (var cmd = _avalonContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @$"
UPDATE game SET numplayers = numplayers - 1
WHERE gameid = {game.GameId} RETURNING numplayers";
                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    game.NumPlayers = (int)result;
                }
            }
        }

        public Game GetGame(long gameId)
        {
            return _avalonContext.Games
                .Include(g => g.Quests)
                .FirstOrDefault(g => g.GameId == gameId);
        }

        public Game GetGame(string publicGameId)
        {
            return _avalonContext.Games
                .Include(g => g.Quests)
                .FirstOrDefault(g => g.PublicId == publicGameId);
        }

        public IQueryable<Game> GetGames()
        {
            return _avalonContext.Games;
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

        public void IncrementPlayerCount(Game game)
        {
            using (var cmd = _avalonContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @$"
UPDATE game SET numplayers = numplayers + 1
WHERE gameid = {game.GameId} RETURNING numplayers";
                if (cmd.Connection.State != System.Data.ConnectionState.Open) cmd.Connection.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    game.NumPlayers = (int)result;
                }
            }
        }

        public void MarkGameAsInactive(long gameId)
        {
            var game = _avalonContext.Games
                .Include(g => g.Players)
                .FirstOrDefault(g => g.Active && g.GameId == gameId);
            if (game == default(Game))
            {
                return;
            }
            game.Active = false;
            foreach (Player player in game.Players)
            {
                player.Disconnected = true;
            }
            _avalonContext.Update(game);
            _avalonContext.SaveChanges();
        }

        public void SetHostUsername(long gameId, string hostUsername)
        {
            _avalonContext.Database.ExecuteSqlInterpolated($"UPDATE game SET hostusername = {hostUsername} WHERE gameid = {gameId}");
        }

        public void UpdateGame(Game game)
        {
            _avalonContext.Games.Update(game);
            _avalonContext.SaveChanges();
        }
    }
}
