using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using online_avalon_web.Core.Interfaces.Workers;

namespace online_avalon_web.Workers
{
    public class GameCleanupQueue : IGameCleanupQueue
    {
        private readonly ConcurrentQueue<long> _queue;
        public GameCleanupQueue()
        {
            _queue = new ConcurrentQueue<long>();
        }

        public void Enqueue(long gameId)
        {
            _queue.Enqueue(gameId);
        }

        public void EnqueueRange(IEnumerable<long> gameIds)
        {
            foreach (long gameId in gameIds)
            {
                _queue.Enqueue(gameId);
            }
        }

        public bool TryDequeue(out long gameId)
        {
            return _queue.TryDequeue(out gameId);
        }
    }
}
