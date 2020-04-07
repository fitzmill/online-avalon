import { Commit } from 'vuex';
import { StartGameDto, QuestStage, NewQuestInfoDto } from '@/types';
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
} from './mutation-types';

const registerSignalREventHandlers = (connection: signalR.HubConnection, commit: Commit) => {
  connection.on('ReceiveNewPlayer', (username: string, message: string) => {
    commit(AddPlayerToGame, username);
    commit(SetServerMessage, message);
  });
  connection.on('StartGame', (initialGameData: StartGameDto) => {
    commit(SetInitialGameData, initialGameData);
    commit(SetQuestStage, QuestStage.ChooseParty);
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
  connection.on('ReceiveUserApprovalVotes', (userApprovalVotes: { [key: string]: string }) => {
    commit(SetUserApprovalVotes, userApprovalVotes);
  });
  connection.on('ReceiveQuestVotes', (questVotes: string[]) => {
    commit(SetQuestVotes, questVotes);
  });
  connection.on('MoveToLakeStage', () => {
    commit(SetQuestStage, QuestStage.Lake);
  });
  connection.on('ReceiveUsernamesToLake', (usernamesToLake: string[]) => {
    commit(SetUsernamesToLake, usernamesToLake);
  });
  connection.on('ReceiveNewQuestInfo', (newQuestInfo: NewQuestInfoDto) => {
    commit(SetNewQuestInfo, newQuestInfo);
    commit(SetQuestStage, QuestStage.ChooseParty);
  });
  connection.on('MoveToAssassinationStage', () => {
    commit(SetQuestStage, QuestStage.Assassinate);
  });
  connection.on('ReceiveUsernamesToAssassinate', (usernamesToAssassinate: string[]) => {
    commit(SetUsernamesToAssassinate, usernamesToAssassinate);
  });
  connection.on('EndGameAndReceiveSummary', (summary) => {
    commit(SetGameSummary, summary);
    commit(SetQuestStage, QuestStage.End);
  });
  connection.on('ReceiveLakeAlignment', (lakedUserAlignment: string) => {
    commit(SetLakedUserAlignment, lakedUserAlignment);
  });
};

export default registerSignalREventHandlers;
