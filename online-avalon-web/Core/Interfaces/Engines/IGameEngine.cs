using System;
using System.Collections.Generic;
using online_avalon_web.Core.Enums;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Engines
{
    public interface IGameEngine
    {
        /// <summary>
        /// Tries to create a new game
        /// </summary>
        /// <param name="hostUsername"></param>
        /// <param name="publicGameId"></param>
        /// <param name="game">Game if the game is created, null otherwise</param>
        /// <returns>True if the game can be created, false if the game already exists and is currently active</returns>
        bool TryCreateGame(string hostUsername, string publicGameId, out Game game);

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        /// <param name="username"></param>
        /// <param name="publicGameId"></param>
        /// <exception cref="InvalidOperationException">If the game has already started</exception>
        /// <exception cref="ArgumentException">If there is already someone in the game with the passed in username</exception>
        /// <returns>The game metadata</returns>
        PlayerGameStatus AddPlayerToGame(string username, string publicGameId);

        /// <summary>
        /// Removes a player from a game
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="username"></param>
        void RemovePlayerFromGame(long gameId, string username, out string newHostUsername);

        /// <summary>
        /// Starts a game and returns custom metadata for each player based on their role.
        /// </summary>
        /// <param name="publicGameId"></param>
        /// <param name="optionalRoles">What optional roles have been added to the game</param>
        /// <returns>custom metadata for each player based on their role</returns>
        Dictionary<string, PlayerGameStatus> StartGame(string publicGameId, IEnumerable<RoleEnum> optionalRoles);

        /// <summary>
        /// Checks if the required number of players have been added to the party
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns>True if the correct number of players are in the party, false otherwise</returns>
        bool TryMoveToApprovalStage(long gameId, out IEnumerable<string> partyUsernames);

        /// <summary>
        /// Checks if all players have voted to approve/disapprove of the party.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="userVotes">Map of usernames to vote</param>
        /// <returns>True if all players have voted, false otherwise</returns>
        bool TryToApproveParty(long gameId, out Dictionary<string, ApprovalVoteOptionsEnum> userVotes, out string newKingUsername);

        /// <summary>
        /// Checks if 5 parties have failed
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        bool HaveFivePartiesFailed(long gameId);

        /// <summary>
        /// Checks if all players have voted to succeed/fail the quest
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="questVotes">Randomized list of the quest votes</param>
        /// <returns>True if all members of the party have voted, false otherwise</returns>
        bool TryCompleteQuest(long gameId, out List<QuestVoteOptionsEnum> questVotes);

        /// <summary>
        /// Checks if the current round is one where a user gets laked
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="usernameWithLake">Who currently has the lake</param>
        /// <param name="usernamesToLake">Who can be laked</param>
        /// <returns>True if the current round is a lake round, false otherwise</returns>
        bool TryMoveToLakeStage(long gameId, out string usernameWithLake, out IEnumerable<string> usernamesToLake);

        /// <summary>
        /// Checks if the game can move to the next quest.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="updatedGame">The updated game state</param>
        /// <returns>True if the game still has at least one quest to go, false if the game is starting to end</returns>
        bool TryMoveToNextQuest(long gameId, out PlayerGameStatus updatedGame);

        /// <summary>
        /// Checks if the game is in the assassination stage
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="usernamesToAssassinate">Usernames that can be assassinated</param>
        /// <returns>True if someone needs to be assassinated, false otherwise</returns>
        bool TryMoveToAssassinationStage(long gameId, out string assassin, out IEnumerable<string> usernamesToAssassinate);

        /// <summary>
        /// Sets the game status to finished
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="gameResult">The result of the game</param>
        /// <returns>All information about the game</returns>
        Game EndGame(long gameId, GameResultEnum gameResult);

        /// <summary>
        /// Checks to see if evil has won 3 quests. Returns true if evil has won, false otherwise
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="gameSummary"></param>
        /// <returns></returns>
        bool TryEvilWins(long gameId, out Game gameSummary);

        /// <summary>
        /// Restarts a finished game and puts it into the pregame mode
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns>The new game</returns>
        Game RestartGame(long gameId);

        /// <summary>
        /// Gets a game with all details
        /// </summary>
        /// <param name="publicGameId"></param>
        /// <returns></returns>
        Game GetGame(string publicGameId);

        Game GetGame(long gameId);

        void MarkGameAsInactive(long gameId);

        IEnumerable<long> GetGameIdsToDeactivate();
    }
}
