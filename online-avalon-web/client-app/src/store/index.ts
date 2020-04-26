import Vue from 'vue';
import Vuex from 'vuex';
import createPersistedState from 'vuex-persistedstate';
import {
  StartGameDto,
  NewQuestInfoDto,
  CreateGameOptions,
  InitialGameDto,
  QuestStage,
  Player,
  QuestResult,
  Role,
  GameSummary,
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
  ContinueQuestAfterLake,
  RestartGame,
  DisconnectFromServer,
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
  SetKingUsername,
  SetLakedUsername,
  IncrementPartyNumber,
  ClearPlayers,
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
  PartyMembers,
  HasLake,
  PlayerWithLake,
  IsConnectedToServer,
} from './getter-types';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    isHost: false,
    questNumber: 0,
    partyNumber: 0,
    username: '',
    publicGameId: '',
    playerRole: Role.Default,
    serverMessage: '',
    serverErrorMessage: '',
    lakedUserAlignment: '',
    lakedUsername: '',
    questStage: QuestStage.Default,
    questVotes: [] as string[],
    usernamesToLake: [] as string[],
    usernamesToAssassinate: [] as string[],
    knownUsernames: [] as string[],
    questResults: [
      QuestResult.Unknown,
      QuestResult.Unknown,
      QuestResult.Unknown,
      QuestResult.Unknown,
      QuestResult.Unknown,
    ] as QuestResult[],
    players: [] as Player[],
    userApprovalVotes: {} as { [key: string]: string },
    gameSummary: {} as GameSummary,
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
    [PartyMembers]: (state) => state.players.filter((p) => p.isInParty),
    [HasLake]: (state) => state.players.find((p) => p.hasLake)?.username === state.username,
    [PlayerWithLake]: (state) => state.players.find((p) => p.hasLake),
    [IsConnectedToServer]: (state) => state.connection?.state === HubConnectionState.Connected,
  },
  mutations: {
    [ClearGameState]: (state) => {
      state.questNumber = 0;
      state.partyNumber = 0;
      state.playerRole = Role.Default;
      state.questStage = QuestStage.Default;
      state.lakedUserAlignment = '';
      state.lakedUsername = '';
      state.knownUsernames = [];
      state.questVotes = [];
      state.usernamesToLake = [];
      state.usernamesToAssassinate = [];
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
    [ClearPlayers]: (state) => {
      state.players = [];
    },
    [SetInitialGameData]: (state, initialGameData: StartGameDto) => {
      state.playerRole = initialGameData.playerRole;
      state.knownUsernames = initialGameData.knownUsernames;
      state.partyNumber = 1;
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
      state.partyNumber = 1;
      state.usernamesToAssassinate = [];
      state.usernamesToLake = [];
      state.lakedUserAlignment = '';
      state.lakedUsername = '';

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
      if (state.questVotes.some((v) => v === 'Fail')) {
        result = QuestResult.EvilWins;
      } else {
        result = QuestResult.GoodWins;
      }
      console.log(result);
      state.questResults.splice(state.questNumber - 1, 1, result);
    },
    [SetKingUsername]: (state, username: string) => {
      const oldKing = state.players.find((p) => p.isKing);
      if (oldKing) {
        oldKing.isKing = false;
      }
      const newKing = state.players.find((p) => p.username === username);
      if (newKing) {
        newKing.isKing = true;
      }
    },
    [SetLakedUsername]: (state, username: string) => {
      state.lakedUsername = username;
    },
    [IncrementPartyNumber]: (state) => {
      state.partyNumber += 1;
    },
    [BuildConnection]: (state) => {
      state.connection = new HubConnectionBuilder()
        .withUrl(`/hubs/game?username=${state.username}&publicGameId=${state.publicGameId}`)
        .withAutomaticReconnect()
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
      commit(ClearGameState);
      commit(ClearPlayers);
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
      commit(ClearGameState);
      commit(ClearPlayers);
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
    [ContinueQuestAfterLake]: async ({ state }) => {
      await state.connection.invoke('ContinueEndQuestAfterLake');
    },
    [LakePlayer]: async ({ state }, username: string) => {
      await state.connection.invoke('LakePlayer', username);
    },
    [AssassinatePlayer]: async ({ state }, username: string) => {
      await state.connection.invoke('AssassinatePlayer', username);
    },
    [RestartGame]: async ({ state }) => {
      await state.connection.invoke('RestartGame');
    },
    [DisconnectFromServer]: async ({ state }) => {
      await state.connection.stop();
    },
  },
  modules: {},
  plugins: [
    createPersistedState({
      storage: window.sessionStorage,
      reducer: (state) => Object.assign({}, ...Object.keys(state)
        .filter((key) => key !== 'connection')
        .map((key) => ({ [key]: state[key] }))),
    }),
  ],
});
