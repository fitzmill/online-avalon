import Vue from 'vue';
import Vuex from 'vuex';
import { InitialGameDto, NewQuestInfoDto, CreateGameOptions } from '@/types';
import signalr from '@microsoft/signalr';
import axios from 'axios';
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
} from './mutation-types';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    isHost: false,
    questNumber: 0,
    gameId: 0,
    username: '',
    publicGameId: '',
    usernameWithLake: '',
    kingUsername: '',
    playerRole: '',
    serverMessage: '',
    serverErrorMessage: '',
    lakedUserAlignment: '',
    partyUsernames: [] as string[],
    questVotes: [] as string[],
    usernamesToLake: [] as string[],
    usernamesToAssassinate: [] as string[],
    knownUsernames: [] as string[],
    players: [] as string[],
    userApprovalVotes: {} as { [key: string]: string },
    gameSummary: {},
    connection: new signalr.HubConnectionBuilder()
      .withUrl('/hubs/game')
      .build() as signalr.HubConnection,
  },
  mutations: {
    [ClearGameState]: (state) => {
      state.isHost = false;
      state.gameId = 0;
      state.questNumber = 0;
      state.username = '';
      state.publicGameId = '';
      state.usernameWithLake = '';
      state.kingUsername = '';
      state.playerRole = '';
      state.serverMessage = '';
      state.lakedUserAlignment = '';
      state.knownUsernames = [];
      state.partyUsernames = [];
      state.questVotes = [];
      state.usernamesToLake = [];
      state.usernamesToAssassinate = [];
      state.players = [];
      state.userApprovalVotes = {};
      state.gameSummary = {};
    },
    [SetInitialGameData]: (state, initialGameData: InitialGameDto) => {
      Object.assign(state, initialGameData);
    },
    [AddPlayerToParty]: (state, username) => {
      state.partyUsernames.push(username);
    },
    [RemovePlayerFromParty]: (state, username) => {
      state.partyUsernames.splice(state.partyUsernames.indexOf(username), 1);
    },
    [SetPartyUsernames]: (state, usernames) => {
      state.partyUsernames = usernames;
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
      Object.assign(state, newQuestInfo);
    },
    [SetUsernamesToAssassinate]: (state, usernamesToAssassinate) => {
      state.usernamesToAssassinate = usernamesToAssassinate;
    },
    [SetGameSummary]: (state, gameSummary) => {
      state.gameSummary = gameSummary;
    },
    [AddPlayerToGame]: (state, username) => {
      state.players.push(username);
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
    [BuildConnection]: (state) => {
      state.connection = new signalr.HubConnectionBuilder()
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
      } catch (error) {
        if (error.response) {
          commit(SetServerErrorMessage, error.response.data);
        }
      }
    },
    [StartGame]: async ({ state }, createGameOptions: CreateGameOptions) => {
      await state.connection.invoke('StartGame', createGameOptions);
    },
    [JoinGame]: async ({ state, dispatch }) => {
      await dispatch(StartConnection);
      await state.connection.invoke('JoinGame', state.publicGameId, state.username);
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
