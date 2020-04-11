export interface StartGameDto {
    playerRole: string;
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
