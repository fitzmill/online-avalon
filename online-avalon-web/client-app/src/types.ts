export interface StartGameDto {
    playerRole: string;
    gameId: number;
    knownUsernames: string[];
    usernameWithLake: string;
    king: string;
}

export interface InitialGameDto {
    hostUsername: string;
    players: [{
        username: string;
    }];
}

export interface NewQuestInfoDto {
    kingUsername: string;
    usernameWithLake: string;
    newQuestNumber: number;
}

export enum QuestStage {
    ChooseParty = 1,
    ApproveParty,
    VoteQuest,
    Lake,
    Assassinate,
    End
}

export interface CreateGameOptions {
    optionalRoles: string[];
}
