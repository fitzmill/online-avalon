export interface StartGameDto {
    playerRole: Role;
    knownUsernames: string[];
    usernameWithLake: string;
    kingUsername: string;
}

export interface InitialGameDto {
    hostUsername: string;
    kingUsername: string;
    partyNumber: number;
    questNumber: number;
    usernameWithLake: string;
    quests: [{
        questResult: 'GoodWins' | 'EvilWins' | null;
    }];
    questStage: QuestStage;
    players: Player[];
}

export interface NewQuestInfoDto {
    kingUsername: string;
    usernameWithLake: string;
    newQuestNumber: number;
}

export enum QuestStage {
    Default = 'Default',
    ChooseParty = 'ChooseParty',
    ApproveParty = 'ApproveParty',
    VoteQuest = 'VoteQuest',
    Lake = 'Lake',
    Assassinate = 'Assassinate',
    End = 'End'
}

export interface Player {
    username: string;
    hasLake: boolean;
    isKing: boolean;
    isInParty: boolean;
    role: Role;
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
    Good = 'Good',
    Evil = 'Evil'
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

export enum ApprovalVoteOptions {
    Approve = 'Approve',
    Reject = 'Reject'
}

export interface GameSummary {
    gameResult?: GameResult;
    players?: Player[];
}

export enum GameResult {
    GoodWins = 'GoodWins',
    EvilWins = 'EvilWins'
}
