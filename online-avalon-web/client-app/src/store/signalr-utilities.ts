import { Commit } from 'vuex';
import { InitialGameDto, QuestStage } from '@/types';
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
} from './mutation-types';

const registerSignalREventHandlers = (connection: signalR.HubConnection, commit: Commit) => {
  connection.on('ReceiveNewPlayer', (username: string, message: string) => {
    commit(AddPlayerToGame, username);
    commit(SetServerMessage, message);
  });
  connection.on('StartGame', (initialGameData: InitialGameDto) => {
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
};

export default registerSignalREventHandlers;
