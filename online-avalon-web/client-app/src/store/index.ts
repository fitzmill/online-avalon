import Vue from 'vue';
import Vuex from 'vuex';
import { InitialGameDto, NewQuestInfoDto } from '@/types';
import signalr from '@microsoft/signalr';
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
} from './mutation-types';
import { StartConnection, StartGame } from './action-types';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    questNumber: 0,
    gameId: 0,
    username: '',
    publicGameId: '',
    usernameWithLake: '',
    kingUsername: '',
    playerRole: '',
    serverMessage: '',
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
      state.username = '';
      state.publicGameId = '';
      state.usernameWithLake = '';
      state.kingUsername = '';
      state.gameId = 0;
      state.knownUsernames = [];
      state.playerRole = '';
      state.serverMessage = '';
      state.partyUsernames = [];
      state.userApprovalVotes = {};
      state.questVotes = [];
      state.usernamesToLake = [];
      state.usernamesToAssassinate = [];
      state.gameSummary = {};
      state.lakedUserAlignment = '';
      state.players = [];
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
    [BuildConnection]: (state) => {
      state.connection = new signalr.HubConnectionBuilder()
        .withUrl(`/hubs/game?username=${state.username}&publicGameId=${state.publicGameId}`)
        .build();
    },
  },
  actions: {
    [StartConnection]: async ({ commit, state }) => {
      commit(BuildConnection);
      await state.connection.start();
    },
    [StartGame]: async ({ state }) => {
      state.connection.invoke('StartGame');
    },
  },
  modules: {},
});
