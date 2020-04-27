using System;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Workers
{
    public interface IGameCleanupQueue
    {
        /// <summary>
        /// Enqueue a gameId to be cleaned up
        /// </summary>
        /// <param name="gameId"></param>
        void Enqueue(long gameId);

        /// <summary>
        /// Tries to get the next item in the queue. Returns true if gameId is a valid gameId
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        bool TryDequeue(out long gameId);
    }
}
