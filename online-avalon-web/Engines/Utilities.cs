using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Engines
{
    public static class Utilities
    {
        public static AlignmentEnum GetAlignmentForRole(RoleEnum role)
        {
            switch (role)
            {
                case RoleEnum.Merlin:
                case RoleEnum.Percival:
                    return AlignmentEnum.Good;
                case RoleEnum.Assassin:
                case RoleEnum.Mordred:
                case RoleEnum.Morgana:
                case RoleEnum.Oberon:
                case RoleEnum.MinionOfMordred:
                    return AlignmentEnum.Evil;
                default:
                    return AlignmentEnum.Good;
            }
        }

        public static int MinNumberOfPlayers = 5;
        public static int MaxNumberOfPlayers = 10;

        public static (int, int) GetNumGoodBadPlayersInGame(int playerCount)
        {

            switch (playerCount)
            {
                case 5:
                    return (3, 2);
                case 6:
                    return (4, 2);
                case 7:
                    return (4, 3);
                case 8:
                    return (5, 3);
                case 9:
                    return (6, 3);
                case 10:
                    return (6, 4);
                default:
                    throw new ArgumentOutOfRangeException($"Argument {nameof(playerCount)} is outside the range of {MinNumberOfPlayers} to {MaxNumberOfPlayers}");
            }
        }

        public static IEnumerable<string> GetKnownUsernamesForPlayer(Player player, IEnumerable<Player> players)
        {
            IEnumerable<Player> knownPlayers = null;
            var role = player.Role;
            switch (role)
            {
                case RoleEnum.Merlin:
                    knownPlayers = players.Where(p =>
                        p.Role == RoleEnum.Assassin ||
                        p.Role == RoleEnum.Morgana ||
                        p.Role == RoleEnum.MinionOfMordred
                    );
                    break;
                case RoleEnum.Assassin:
                    knownPlayers = players.Where(p =>
                        p.Role == RoleEnum.MinionOfMordred ||
                        p.Role == RoleEnum.Mordred ||
                        p.Role == RoleEnum.Morgana
                    );
                    break;
                case RoleEnum.Morgana:
                    knownPlayers = players.Where(p =>
                        p.Role == RoleEnum.Assassin ||
                        p.Role == RoleEnum.Mordred ||
                        p.Role == RoleEnum.MinionOfMordred
                    );
                    break;
                case RoleEnum.Mordred:
                    knownPlayers = players.Where(p =>
                        p.Role == RoleEnum.Morgana ||
                        p.Role == RoleEnum.Assassin ||
                        p.Role == RoleEnum.MinionOfMordred
                    );
                    break;
                case RoleEnum.MinionOfMordred:
                    knownPlayers = players.Where(p =>
                        p.Role == RoleEnum.MinionOfMordred ||
                        p.Username != player.Username ||
                        p.Role == RoleEnum.Assassin ||
                        p.Role == RoleEnum.Mordred ||
                        p.Role == RoleEnum.Morgana
                    );
                    break;
                case RoleEnum.Percival:
                    knownPlayers = players.Where(p =>
                        p.Role == RoleEnum.Merlin ||
                        p.Role == RoleEnum.Morgana
                    );
                    break;
            }

            return knownPlayers.Select(p => p.Username);
        }

        public static int GetRequiredQuestVotes(int questNumber, int playerCount)
        {
            if (playerCount < MinNumberOfPlayers || playerCount > MaxNumberOfPlayers)
            {
                throw new ArgumentOutOfRangeException($"Argument {nameof(playerCount)} is outside the range of {MinNumberOfPlayers} to {MaxNumberOfPlayers}");
            }

            if (playerCount == 5)
            {
                switch (questNumber)
                {
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    case 3:
                        return 2;
                    case 4:
                        return 3;
                    case 5:
                        return 3;
                    default:
                        throw new ArgumentOutOfRangeException($"Argument {nameof(questNumber)} is not in the range 1-5");
                }
            }
            else if (playerCount == 6)
            {
                switch (questNumber)
                {
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    case 3:
                        return 4;
                    case 4:
                        return 3;
                    case 5:
                        return 4;
                    default:
                        throw new ArgumentOutOfRangeException($"Argument {nameof(questNumber)} is not in the range 1-5");
                }
            }
            else if (playerCount == 7)
            {
                switch (questNumber)
                {
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    case 3:
                        return 3;
                    case 4:
                        return 4;
                    case 5:
                        return 4;
                    default:
                        throw new ArgumentOutOfRangeException($"Argument {nameof(questNumber)} is not in the range 1-5");
                }
            }
            else if (playerCount == 8)
            {
                switch (questNumber)
                {
                    case 1:
                        return 3;
                    case 2:
                        return 4;
                    case 3:
                        return 4;
                    case 4:
                        return 5;
                    case 5:
                        return 5;
                    default:
                        throw new ArgumentOutOfRangeException($"Argument {nameof(questNumber)} is not in the range 1-5");
                }
            }
            else if (playerCount == 9)
            {
                switch (questNumber)
                {
                    case 1:
                        return 3;
                    case 2:
                        return 4;
                    case 3:
                        return 4;
                    case 4:
                        return 5;
                    case 5:
                        return 5;
                    default:
                        throw new ArgumentOutOfRangeException($"Argument {nameof(questNumber)} is not in the range 1-5");
                }
            }
            else
            {
                switch (questNumber)
                {
                    case 1:
                        return 3;
                    case 2:
                        return 4;
                    case 3:
                        return 4;
                    case 4:
                        return 5;
                    case 5:
                        return 5;
                    default:
                        throw new ArgumentOutOfRangeException($"Argument {nameof(questNumber)} is not in the range 1-5");
                }
            }
        }

        public static Random random
        {
            get
            {
                return ThreadSafeRandom.ThisThreadsRandom;
            }
        }

        public static List<int> GetRandomSequence(int size)
        {
            /**
             * The following random sequence generator was found in the following post
             * https://codereview.stackexchange.com/a/61372
             */

            HashSet<int> indicesHash = new HashSet<int>(size);

            for (int top = -1; top < size; top++)
            {
                if (!indicesHash.Add(random.Next(0, top + 1)))
                {
                    indicesHash.Add(top);
                }
            }

            List<int> shuffledList = indicesHash.ToList();

            for (int i = size - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int tmp = shuffledList[k];
                shuffledList[k] = shuffledList[i];
                shuffledList[i] = tmp;
            }

            return shuffledList;
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            var randomIndices = GetRandomSequence(list.Count);

            return list.OrderBy((e) => randomIndices[list.IndexOf(e)]).ToList();
        }
    }

    /// <summary>
    /// This is a thread safe random object that can be called from static classes
    /// Found in this SO post https://stackoverflow.com/a/1262619
    /// </summary>
    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
