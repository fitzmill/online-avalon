import { Commit, Dispatch } from 'vuex';
import {
  QuestStage, NewQuestInfoDto, InitialGameDto, Player,
} from '@/types';
import { formatUserLeavingMessage, formatUserJoiningMessage } from '@/Utility';
import {
  AddPlayerToGame,
  SetServerMessage,
  SetInitialGameData,
  AddPlayerToParty,
  RemovePlayerFromParty,
  SetPartyUsernames,
  SetUserApprovalVotes,
  SetQuestVotes,
  SetQuestStage,
  SetUsernamesToLake,
  SetNewQuestInfo,
  SetUsernamesToAssassinate,
  SetGameSummary,
  SetLakedUserAlignment,
  SetCurrentQuestResult,
  SetKingUsername,
  SetLakedUsername,
  ClearGameState,
  RemovePlayerFromGame,
  SetHostUsername,
  SetNextQuestStage,
} from './mutation-types';
import { ResetConnection } from './action-types';

const registerSignalREventHandlers = (
  connection: signalR.HubConnection,
  commit: Commit,
  dispatch: Dispatch,
) => {
  connection.on('ReceiveNewPlayer', (player: Player) => {
    commit(AddPlayerToGame, player);
    commit(SetServerMessage, formatUserJoiningMessage(player.username));
  });
  connection.on('StartGame', (initialGameData: InitialGameDto) => {
    commit(SetInitialGameData, initialGameData);
  });
  connection.on('AddPlayerToParty', (username: string) => {
    commit(AddPlayerToParty, username);
  });
  connection.on('RemovePlayerFromParty', (username: string) => {
    commit(RemovePlayerFromParty, username);
  });
  connection.on('StartApprovalVote', (partyUsernames: string[]) => {
    commit(SetPartyUsernames, partyUsernames);
    commit(SetQuestStage, QuestStage.ApproveParty);
  });
  connection.on('ReceiveUserApprovalVotes',
    (userApprovalVotes: { [key: string]: string }, newKingUsername: string, summary) => {
      commit(SetUserApprovalVotes, userApprovalVotes);
      if (newKingUsername) {
        commit(SetKingUsername, newKingUsername);
      }
      if (summary) {
        commit(SetGameSummary, summary);
      }
    });
  connection.on('ReceiveQuestVotes', (questVotes: string[]) => {
    commit(SetQuestVotes, questVotes);
    commit(SetCurrentQuestResult);
  });
  connection.on('MoveToLakeStage', () => {
    commit(SetNextQuestStage, QuestStage.Lake);
  });
  connection.on('ReceiveUsernamesToLake', (usernamesToLake: string[]) => {
    commit(SetUsernamesToLake, usernamesToLake);
  });
  connection.on('ReceiveLakedUsername', (username: string) => {
    commit(SetLakedUsername, username);
  });
  connection.on('ReceiveNewQuestInfo', (newQuestInfo: NewQuestInfoDto) => {
    commit(SetNewQuestInfo, newQuestInfo);
    commit(SetNextQuestStage, QuestStage.ChooseParty);
  });
  connection.on('MoveToAssassinationStage', () => {
    commit(SetNextQuestStage, QuestStage.Assassinate);
  });
  connection.on('ReceiveUsernamesToAssassinate', (usernamesToAssassinate: string[]) => {
    commit(SetUsernamesToAssassinate, usernamesToAssassinate);
  });
  connection.on('EndGameAndReceiveSummary', (summary) => {
    commit(SetGameSummary, summary);
    commit(SetNextQuestStage, QuestStage.End);
  });
  connection.on('ReceiveLakeAlignment', (lakedUserAlignment: string) => {
    commit(SetLakedUserAlignment, lakedUserAlignment);
  });
  connection.on('ResetGame', () => {
    commit(ClearGameState);
    dispatch(ResetConnection);
  });
  connection.on('ReceiveDisconnectedPlayer', (username: string, newHostUsername: string) => {
    commit(RemovePlayerFromGame, username);
    if (newHostUsername) {
      commit(SetHostUsername, newHostUsername);
    }
    commit(SetServerMessage, formatUserLeavingMessage(username, newHostUsername));
  });
};

export default registerSignalREventHandlers;
