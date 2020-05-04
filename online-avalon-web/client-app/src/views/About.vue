<template>
  <div class="uk-container uk-margin-small-top uk-text-left">
    <h3>What is this?</h3>
    <p>
      This is an online, open-source version of the board game The Resistance: Avalon. <br />
      You can view the GitHub repository <a href="https://github.com/fitzmill/online-avalon">here</a>.
    </p>
    <h3>How do I play?</h3>
    <p>
      On the home page, you create a game by hitting "Create Game" and entering a name for
      your game room and giving yourself a username.
    </p>
    <p>
      You'll get an error if there's a live game with the same room name as the one you're
      trying to create so make sure that yours is unique.
    </p>
    <p>
      Once you've created a game, tell your friends to join using the room name that you
      used to create the game. You'll see them pop up on your screen as they join in.
      While you're waiting, you can set the game options.
    </p>
    <h4>Game Options</h4>
    <p>
      For now, the only game option you can set is what optional roles will be assigned
      for your game.
    </p>
    <h5>Optional Roles</h5>
    <ul class="uk-list">
      <li>
        <strong>Percival</strong>:
        Percival is good. He knows the identity of Merlin in games without Morgana.
        If Morgana is in the game Percival is "aware" of both Merlin and Morgana,
        but isn't aware of who is Merlin and who is Morgana.
      </li>
      <li>
        <strong>Morgana</strong>:
        Morgana is evil. She appears to be a possible Merlin to Percival.
      </li>
      <li>
        <strong>Mordred</strong>:
        Mordred is evil. Merlin is unaware that he is evil.
      </li>
      <li>
        <strong>Oberon</strong>:
        Oberon is evil. No one knows of his identity and he does not know of
        anyone else's identity.
      </li>
    </ul>
    <h4>Starting the Game</h4>
    <p>
      Once all players have joined your game, the host can select to start the game.<br />
      Starting the game will randomly assign players to roles, and choose a random king.
      Players can see their role and who was assigned the <strong>Lake</strong> by clicking
      the <strong>Player Info</strong> button.
    </p>
    <h4>Choosing the Party</h4>
    <p>
      The king can choose their party, and the rest of the players can see who the king
      is choosing. Once the king has chosen the party everyone can vote whether the party
      should be approved or not. Players can see who voted to approve and disapprove the quest.
      <br />
      If less than half of the players approve of the party,
      a new king is chosen and they can choose a new party. If five parties are disapproved
      in one quest, Evil automatically wins the game. Once a party is approved by half
      or more of the players, the game moves on to the quest stage.
    </p>
    <h4>Going on the Quest</h4>
    <p>
      The players in the party decide whether the quest should succeed or fail.
      If a single player decides to Fail the quest, the entire quest fails. Players can't
      see who decided to Succeed or Fail the quest and the results are displayed in a
      random order.
    </p>
    <h4>Laking a Player</h4>
    <p>
      At the start of the game, a random player is given the <strong>Lake</strong>. At the
      end of certain quests, the player with the <strong>Lake</strong> can select a player
      to see what that player's alignment is (Good/Evil). Other players can see who was laked,
      but they cannot see what the alignment is. The <strong>Lake</strong> is then given to
      the player who was laked to be used in a future quest.
    </p>
    <h4>Ending the Game</h4>
    <p>
      If three quests fail, Evil automatically wins. If three quests succeed, the
      <strong>Assassin</strong> gets a chance to choose who they think is Merlin.
      If the <strong>Assassin</strong> chooses correctly, Evil wins the game. Otherwise, Good wins
      the game.
    </p>
  </div>
</template>
