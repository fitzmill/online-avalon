import { Alignment, Role } from './types';

export function getAlignmentForPlayer(playerRole: Role) {
  switch (playerRole) {
    case (Role.Mordred):
    case (Role.Morgana):
    case (Role.Oberon):
    case (Role.MinionOfMordred):
    case (Role.Assassin):
      return Alignment.Evil;
    case (Role.Merlin):
    case (Role.Percival):
    case (Role.LoyalServantOfArthur):
      return Alignment.Good;
    default:
      return Alignment.Good;
  }
}

export function getPlayerDisplayText(playerRole: Role, knownUsernames: string[]) {
  if (playerRole === Role.LoyalServantOfArthur) {
    return 'You are a loyal servant of Arthur. Do your best to help good win.';
  }
  if (playerRole === Role.Merlin) {
    if (knownUsernames.length === 1) {
      return `You are Merlin. You know that ${knownUsernames[0]} is evil`;
    }
    let str = 'You are Merlin. You know that ';
    for (let i = knownUsernames.length - 1; i > 1; i -= 1) {
      str = str.concat(`${knownUsernames[i]}, `);
    }
    return str.concat(`${knownUsernames[1]} and ${knownUsernames[0]} are evil.`);
  }
  if (playerRole === Role.Percival) {
    if (knownUsernames.length > 1) {
      return `You are Percival. You are aware of ${knownUsernames[0]} and ${knownUsernames[1]} but you don't who is Merlin and who is Morgana.`;
    }
    return `You are Percival. You know that ${knownUsernames[0]} is Merlin.`;
  }
  if (getAlignmentForPlayer(playerRole) === Alignment.Evil && playerRole !== Role.Oberon) {
    let str = '';
    if (playerRole === Role.MinionOfMordred) {
      str = 'You are a Minion of Mordred. You know that ';
    } else if (playerRole === Role.Assassin) {
      str = 'You are the Assassin. You know that ';
    } else {
      str = `You are ${playerRole}. You know that `;
    }
    if (knownUsernames.length === 1) {
      return str.concat(`${knownUsernames[0]} is evil.`);
    }
    for (let i = knownUsernames.length - 1; i > 1; i -= 1) {
      str = str.concat(`${knownUsernames[i]}, `);
    }
    return str.concat(`${knownUsernames[1]} and ${knownUsernames[0]} are also evil.`);
  }
  if (playerRole === Role.Oberon) {
    return 'You are Oberon. You know nothing, wreak havoc as you wish.';
  }
  return '';
}

export function formatServerErrorMessage(errorMessage: string) {
  const index = errorMessage.indexOf(': ');
  if (index > -1) {
    return errorMessage.substr(index + 1);
  }

  return errorMessage;
}

export function formatUserLeavingMessage(username: string, newHostUsername: string) {
  if (newHostUsername) {
    return `${username} has left the game, ${newHostUsername} is now the host`;
  }

  return `${username} has left the game`;
}

export function formatUserJoiningMessage(username: string) {
  return `${username} has joined the game`;
}
