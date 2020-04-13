export interface StartGameDto {
    playerRole: Role;
    knownUsernames: string[];
    usernameWithLake: string;
    king: string;
}

export interface InitialGameDto {
    hostUsername: string;
    players: Player[];
}

export interface NewQuestInfoDto {
    kingUsername: string;
    usernameWithLake: string;
    newQuestNumber: number;
}

export enum QuestStage {
    Default = 0,
    ChooseParty = 1,
    ApproveParty,
    VoteQuest,
    Lake,
    Assassinate,
    End
}

export interface Player {
    username: string;
    isHost: boolean;
    hasLake?: boolean;
    isKing?: boolean;
    isInParty?: boolean;
}

export interface CreateGameOptions {
    optionalRoles: string[];
}

export enum QuestResult {
    Unknown = '?',
    GoodWins = '✔',
    EvilWins = 'X'
}

export enum Alignment {
    Good = 1,
    Evil
}

export enum Role {
    Mordred = 'Mordred',
    Morgana = 'Morgana',
    Oberon = 'Oberon',
    MinionOfMordred = 'MinionOfMordred',
    Assassin = 'Assassin',
    Merlin = 'Merlin',
    Percival = 'Percival',
    LoyalServantOfArthur = 'LoyalServantOfArthur',
    Default = '',
}
