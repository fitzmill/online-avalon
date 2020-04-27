using System;
using System.Collections.Concurrent;
using online_avalon_web.Core.Interfaces.Workers;

namespace online_avalon_web.Workers
{
    public class GameCleanupQueue : IGameCleanupQueue
    {
        private readonly ConcurrentQueue<GameCleanupQueueItem> _queue;
        private readonly int _cleanupDelaySeconds;
        public GameCleanupQueue()
        {
            _queue = new ConcurrentQueue<GameCleanupQueueItem>();
            _cleanupDelaySeconds = 20;
        }

        public void Enqueue(long gameId)
        {
            _queue.Enqueue(new GameCleanupQueueItem(gameId));
        }

        public bool TryDequeue(out long gameId)
        {
            if (_queue.TryPeek(out GameCleanupQueueItem item))
            {
                if (item.AddedToQueue < DateTime.Now - TimeSpan.FromSeconds(_cleanupDelaySeconds))
                {
                    _queue.TryDequeue(out GameCleanupQueueItem item2);
                    gameId = item2.GameId;

                    return true;
                }
            }

            gameId = -1;
            return false;
        }

        private class GameCleanupQueueItem
        {
            public long GameId { get; }
            public DateTime AddedToQueue { get; }

            public GameCleanupQueueItem(long gameId)
            {
                GameId = gameId;
                AddedToQueue = DateTime.Now;
            }
        }
    }
}
