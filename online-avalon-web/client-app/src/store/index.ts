import Vue from 'vue';
import Vuex from 'vuex';
import {
  StartGameDto,
  NewQuestInfoDto,
  CreateGameOptions,
  InitialGameDto,
  QuestStage,
  Player,
  QuestResult,
  Role,
} from '@/types';
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr';
import axios from 'axios';
import { getPlayerDisplayText, getAlignmentForPlayer } from '@/Utility';
import registerSignalREventHandlers from './signalr-utilities';
import apiConfigs from './api-utilities';
import {
  StartConnection,
  StartGame,
  JoinGame,
  CreateGame,
  LeaveGame,
  AddUserToParty,
  RemoveUserFromParty,
  SubmitParty,
  VoteForParty,
  VoteForQuest,
  SendEndQuestInfo,
  LakePlayer,
  AssassinatePlayer,
} from './action-types';
import {
  ClearGameState,
  SetInitialGameData,
  AddPlayerToParty,
  RemovePlayerFromParty,
  SetPartyUsernames,
  SetUserApprovalVotes,
  SetQuestVotes,
  SetUsernamesToLake,
  SetLakedUserAlignment,
  SetNewQuestInfo,
  SetUsernamesToAssassinate,
  SetGameSummary,
  BuildConnection,
  AddPlayerToGame,
  RemovePlayerFromGame,
  SetServerMessage,
  SetServerErrorMessage,
  SetPlayerAsHost,
  SetUsername,
  SetPublicGameId,
  SetHostUsername,
  SetPlayers,
  SetQuestStage,
  SetCurrentQuestResult,
} from './mutation-types';
import {
  IsDefaultStage,
  IsChoosePartyStage,
  IsApprovePartyStage,
  IsVoteQuestStage,
  IsLakeStage,
  IsAssassinateStage,
  IsEndStage,
  IsLeader,
  PartyLeader,
  IsInGame,
  PlayerDisplayText,
  PlayerAlignment,
} from './getter-types';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    isHost: false,
    questNumber: 0,
    username: 'fitzweeb',
    publicGameId: 'test',
    playerRole: Role.LoyalServantOfArthur,
    serverMessage: '',
    serverErrorMessage: '',
    lakedUserAlignment: '',
    questStage: QuestStage.ChooseParty,
    questVotes: [] as string[],
    usernamesToLake: [] as string[],
    usernamesToAssassinate: [] as string[],
    knownUsernames: ['test1', 'test2'] as string[],
    questResults: [
      QuestResult.Unknown,
      QuestResult.Unknown,
      QuestResult.Unknown,
      QuestResult.Unknown,
      QuestResult.Unknown,
    ] as QuestResult[],
    players: [
      { username: 'fitzmill', isHost: true, isInParty: false },
      { username: 'fitzyaskfjlajsklfjlka;djflak;sd;fladsjflkasjf', isInParty: true },
      { username: 'fitzweeb', isKing: true, isInParty: false },
      { username: 'test1' },
      { username: 'test2' },
      { username: 'test3' },
      { username: 'test4' },
      { username: 'test5' },
      { username: 'test6' },
      { username: 'test7' },
    ] as Player[],
    userApprovalVotes: {} as { [key: string]: string },
    gameSummary: {},
    connection: new HubConnectionBuilder()
      .withUrl('/hubs/game')
      .build() as HubConnection,
  },
  getters: {
    [IsDefaultStage]: (state) => state.questStage === QuestStage.Default,
    [IsChoosePartyStage]: (state) => state.questStage === QuestStage.ChooseParty,
    [IsApprovePartyStage]: (state) => state.questStage === QuestStage.ApproveParty,
    [IsVoteQuestStage]: (state) => state.questStage === QuestStage.VoteQuest,
    [IsLakeStage]: (state) => state.questStage === QuestStage.Lake,
    [IsAssassinateStage]: (state) => state.questStage === QuestStage.Assassinate,
    [IsEndStage]: (state) => state.questStage === QuestStage.End,
    [IsLeader]: (state) => state.players.find((p) => p.isKing)?.username === state.username,
    [PartyLeader]: (state) => state.players.find((p) => p.isKing),
    [IsInGame]: (state) => state.connection.state === HubConnectionState.Connected,
    [PlayerDisplayText]: (state) => getPlayerDisplayText(state.playerRole, state.knownUsernames),
    [PlayerAlignment]: (state) => getAlignmentForPlayer(state.playerRole),
  },
  mutations: {
    [ClearGameState]: (state) => {
      state.isHost = false;
      state.questNumber = 0;
      state.username = '';
      state.publicGameId = '';
      state.playerRole = Role.Default;
      state.serverMessage = '';
      state.lakedUserAlignment = '';
      state.knownUsernames = [];
      state.questVotes = [];
      state.usernamesToLake = [];
      state.usernamesToAssassinate = [];
      state.players = [];
      state.userApprovalVotes = {};
      state.gameSummary = {};
      state.questResults = [
        QuestResult.Unknown,
        QuestResult.Unknown,
        QuestResult.Unknown,
        QuestResult.Unknown,
        QuestResult.Unknown,
      ];
    },
    [SetInitialGameData]: (state, initialGameData: StartGameDto) => {
      state.playerRole = initialGameData.playerRole;
      state.knownUsernames = initialGameData.knownUsernames;
      const king = state.players.find((p) => p.username === initialGameData.king);
      if (king) {
        king.isKing = true;
      }
      const lake = state.players.find((p) => p.username === initialGameData.usernameWithLake);
      if (lake) {
        lake.hasLake = true;
      }
    },
    [AddPlayerToParty]: (state, username) => {
      const player = state.players.find((p) => p.username === username);
      if (player) {
        player.isInParty = true;
      }
    },
    [RemovePlayerFromParty]: (state, username) => {
      const player = state.players.find((p) => p.username === username);
      if (player) {
        player.isInParty = false;
      }
    },
    [SetPartyUsernames]: (state, usernames: string[]) => {
      const players = state.players.filter((p) => usernames.indexOf(p.username) !== -1);
      players.forEach((p) => {
        // eslint-disable-next-line no-param-reassign
        p.isInParty = true;
      });
    },
    [SetUserApprovalVotes]: (state, userApprovalVotes: { [key: string]: string }) => {
      state.userApprovalVotes = userApprovalVotes;
    },
    [SetQuestVotes]: (state, questVotes: string[]) => {
      state.questVotes = questVotes;
    },
    [SetUsernamesToLake]: (state, usernamesToLake) => {
      state.usernamesToLake = usernamesToLake;
    },
    [SetLakedUserAlignment]: (state, alignment) => {
      state.lakedUserAlignment = alignment;
    },
    [SetNewQuestInfo]: (state, newQuestInfo: NewQuestInfoDto) => {
      state.players.forEach((p) => {
        /* eslint-disable no-param-reassign */
        p.hasLake = false;
        p.isInParty = false;
        p.isKing = false;
        /* eslint-enable no-param-reassign */
      });
      state.questNumber = newQuestInfo.newQuestNumber;
      const king = state.players.find((p) => p.username === newQuestInfo.kingUsername);
      if (king) {
        king.isKing = true;
      }
      const lake = state.players.find((p) => p.username === newQuestInfo.usernameWithLake);
      if (lake) {
        lake.hasLake = true;
      }
    },
    [SetUsernamesToAssassinate]: (state, usernamesToAssassinate) => {
      state.usernamesToAssassinate = usernamesToAssassinate;
    },
    [SetGameSummary]: (state, gameSummary) => {
      state.gameSummary = gameSummary;
    },
    [AddPlayerToGame]: (state, username: string) => {
      state.players.push({ username, isHost: false });
    },
    [RemovePlayerFromGame]: (state, username) => {
      state.players.splice(state.players.indexOf(username), 1);
    },
    [SetServerMessage]: (state, message) => {
      state.serverMessage = message;
    },
    [SetServerErrorMessage]: (state, message) => {
      state.serverErrorMessage = message;
    },
    [SetPlayerAsHost]: (state) => {
      state.isHost = true;
    },
    [SetUsername]: (state, username) => {
      state.username = username;
    },
    [SetPublicGameId]: (state, publicGameId) => {
      state.publicGameId = publicGameId;
    },
    [SetHostUsername]: (state, username) => {
      const player = state.players.find((p) => p.username === username);
      if (player) {
        player.isHost = true;
      }
    },
    [SetPlayers]: (state, players) => {
      state.players = players;
    },
    [SetQuestStage]: (state, stage: QuestStage) => {
      state.questStage = stage;
    },
    [SetCurrentQuestResult]: (state) => {
      let result = QuestResult.Unknown;
      if (state.questVotes.some((v) => v === 'EvilWins')) {
        result = QuestResult.EvilWins;
      } else {
        result = QuestResult.GoodWins;
      }
      state.questResults[state.questNumber] = result;
    },
    [BuildConnection]: (state) => {
      state.connection = new HubConnectionBuilder()
        .withUrl(`/hubs/game?username=${state.username}&publicGameId=${state.publicGameId}`)
        .build();
    },
  },
  actions: {
    [StartConnection]: async ({ state, commit }) => {
      commit(BuildConnection);
      registerSignalREventHandlers(state.connection, commit);
      await state.connection.start();
    },
    [CreateGame]: async ({ state, commit, dispatch }) => {
      const data = {
        hostUsername: state.username,
        publicGameId: state.publicGameId,
      };
      try {
        await axios(Object.assign(apiConfigs.createGame, { data }));
        await dispatch(StartConnection);
        await state.connection.invoke('JoinGameAsHost', state.publicGameId);
        commit(SetPlayerAsHost);
        commit(AddPlayerToGame, state.username);
        commit(SetHostUsername, state.username);
      } catch (error) {
        if (error.response) {
          commit(SetServerErrorMessage, error.response.data);
        }
        throw error;
      }
    },
    [StartGame]: async ({ state }, createGameOptions: CreateGameOptions) => {
      await state.connection.invoke('StartGame', createGameOptions);
    },
    [JoinGame]: async ({ state, dispatch, commit }) => {
      await dispatch(StartConnection);
      const data = await state.connection.invoke('JoinGame', state.publicGameId, state.username) as InitialGameDto;
      commit(SetPlayers, data.players.map((p) => p.username));
      commit(SetPlayerAsHost, data.hostUsername);
    },
    [LeaveGame]: async ({ state }) => {
      await state.connection.invoke('LeaveGame');
    },
    [AddUserToParty]: async ({ state }, username: string) => {
      await state.connection.invoke('AddUserToParty', username);
    },
    [RemoveUserFromParty]: async ({ state }, username: string) => {
      await state.connection.invoke('RemoveUserFromParty', username);
    },
    [SubmitParty]: async ({ state }) => {
      await state.connection.invoke('SubmitParty');
    },
    [VoteForParty]: async ({ state }, vote: string) => {
      await state.connection.invoke('VoteForParty', vote);
    },
    [VoteForQuest]: async ({ state }, vote: string) => {
      await state.connection.invoke('VoteForQuest', vote);
    },
    [SendEndQuestInfo]: async ({ state }) => {
      await state.connection.invoke('SendEndQuestInfo');
    },
    [LakePlayer]: async ({ state }, username: string) => {
      await state.connection.invoke('LakePlayer', username);
    },
    [AssassinatePlayer]: async ({ state }, username: string) => {
      await state.connection.invoke('AssassinatePlayer', username);
    },
  },
  modules: {},
});
