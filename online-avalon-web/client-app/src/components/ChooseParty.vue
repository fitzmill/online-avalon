<template>
  <div>
    <div v-if="isLeader">
      <h4>Choose your party</h4>
      <div class="uk-child-width-1-2 uk-grid-small" uk-grid>
        <div v-for="player in players" :key="player.username">
          <button
            class="app-button uk-button-default uk-button-small uk-width-1-1 uk-text-truncate"
            :class="[player.isInParty ? 'in-party' : '']"
            @click="swapPartyStatus(player)"
            :disabled="playerLoadingMap[player.username]">
            {{player.username}}
          </button>
        </div>
      </div>
      <button
        class="uk-button uk-button-success uk-margin-top"
        @click="submitParty()"
        :disabled="submitLoading">
        Submit Party
      </button>
    </div>
    <div v-else>
      <h4>Party members</h4>
      <div class="uk-child-width-1-2 uk-grid-small" uk-grid>
        <div v-for="player in players" :key="player.username">
          <div class="uk-text-truncate"
            :class="player.isInParty ? 'uk-text-emphasis' : 'uk-text-muted'">
            {{player.username}}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import {
  Getter,
  State,
  Action,
  Mutation,
} from 'vuex-class';
import UIkit from 'uikit';
import { Player, QuestStage } from '@/types';
import { IsLeader } from '../store/getter-types';
import { AddUserToParty, RemoveUserFromParty, SubmitParty } from '../store/action-types';
import { AddPlayerToParty, RemovePlayerFromParty, SetQuestStage } from '../store/mutation-types';

@Component
export default class ChooseParty extends Vue {
  @Getter(IsLeader) private isLeader!: boolean;

  @State private players!: Player[];

  @State private serverErrorMessage!: string;

  private partyApproved = false;

  private playerLoadingMap: { [key: string ]: boolean } = {};

  private submitLoading = false;

  @Mutation(AddPlayerToParty) private addPlayerToParty!: (username: string) => void;

  @Mutation(RemovePlayerFromParty) private removePlayerFromParty!: (username: string) => void;

  @Mutation(SetQuestStage) private setQuestStage!: (stage: QuestStage) => void;

  @Action(AddUserToParty) private dispatchAddUserToParty!: (username: string) => Promise<void>

  @Action(RemoveUserFromParty)
    private dispatchRemoveUserFromParty!: (username: string) => Promise<void>;

  @Action(SubmitParty) private dispatchSumbitParty!: () => Promise<void>;

  private async swapPartyStatus(player: Player) {
    this.playerLoadingMap[player.username] = true;
    try {
      if (player.isInParty) {
        this.removePlayerFromParty(player.username);
        await this.dispatchRemoveUserFromParty(player.username);
      } else {
        this.addPlayerToParty(player.username);
        await this.dispatchAddUserToParty(player.username);
      }
    } catch {
      UIkit.notification(this.serverErrorMessage);
    } finally {
      this.playerLoadingMap[player.username] = false;
    }
  }

  private async submitParty() {
    this.submitLoading = true;
    try {
      await this.dispatchSumbitParty();
      this.setQuestStage(QuestStage.ApproveParty);
    } catch {
      UIkit.notification(this.serverErrorMessage);
    } finally {
      this.submitLoading = false;
    }
  }
}
</script>

<style scoped>
.in-party {
  border: 2px solid #1a921a;
}
</style>
