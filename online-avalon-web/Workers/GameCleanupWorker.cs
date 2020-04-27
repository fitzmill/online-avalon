﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Interfaces.Workers;

namespace online_avalon_web.Workers
{
    public class GameCleanupWorker : IHostedService, IDisposable
    {
        private int _executing;
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public GameCleanupWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CleanupGames, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void CleanupGames(object state)
        {
            // other threads aren't executing
            if (0 == Interlocked.Exchange(ref _executing, 1))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var cleanupQueue = scope.ServiceProvider.GetRequiredService<IGameCleanupQueue>();
                    var gameAccessor = scope.ServiceProvider.GetRequiredService<IGameAccessor>();

                    while (cleanupQueue.TryDequeue(out long gameId))
                    {
                        var game = gameAccessor.GetGame(gameId);

                        // make sure game is still inactive
                        if (game.NumPlayers == 0)
                        {
                            game.Active = false;

                            gameAccessor.UpdateGame(game);

                        }
                    }
                }
                // unlock method
                Interlocked.Exchange(ref _executing, 0);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
