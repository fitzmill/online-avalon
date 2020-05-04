<template>
  <div>
    <label id="publicGameId">Lobby: {{publicGameId}}</label>
    <div class="uk-flex uk-flex-center uk-margin-top">
      <WaitingRoom v-if="isDefaultStage" />
      <div v-else>
        <h3>Quest History</h3>
        <div class="uk-grid-divider uk-grid-medium uk-child-width-auto uk-flex-center" uk-grid>
          <div v-for="(result, i) in questResults" :key="`${result}:${i}`"
            :class="[i === questNumber-1 ? 'uk-text-bold' : 'uk-text-muted']">
            {{i+1}}
            <div :class="[getClassForQuestResult(i)]">{{result}}</div>
          </div>
        </div>
        <div class="uk-card uk-card-default uk-width-xlarge@s uk-margin-small-top">
          <div class="uk-card-header">
            <div class="uk-child-width-auto uk-flex-center" uk-grid>
              <div>
                <button class="uk-button uk-button-primary uk-button-small"
                  uk-toggle="target: #player-info-modal">
                  Player Info
                </button>
              </div>
              <div v-if="isHost">
                <button class="uk-button uk-button-default uk-button-small"
                  uk-toggle="target: #game-options-modal">
                  Game Options
                </button>
              </div>
            </div>
            <div v-if="partyLeader" class="uk-margin-small-top">
              {{partyLeader.username}} is the King
            </div>
          </div>
          <div class="uk-card-body">
            <transition
              mode="out-in"
              v-on:before-enter="beforeEnter"
              v-on:enter="enter"
              v-on:leave="leave"
              v-bind:css="false">
              <component v-bind:is="currentComponent"></component>
            </transition>
          </div>
        </div>
      </div>
    </div>

    <!-- Player Info Modal -->
    <div id="player-info-modal" class="uk-flex-top" uk-modal>
      <div class="uk-modal-dialog uk-modal-body uk-margin-auto-vertical">
        <button class="uk-modal-close-default" type="button" uk-close></button>

        <PlayerInfo />

        <button class="uk-button uk-button-primary uk-button-small uk-align-center"
          uk-toggle="target: #player-info-modal">
          Close
        </button>
      </div>
    </div>

    <!-- Game Options Modal -->
    <div id="game-options-modal" class="uk-flex-top" uk-modal v-if="isHost">
      <div class="uk-modal-dialog uk-modal-body uk-margin-auto-vertical uk-width-medium">
        <button class="uk-modal-close-default" type="button" uk-close></button>

        <MidGameOptions />

        <button class="uk-button uk-button-primary uk-button-small uk-align-center"
          uk-toggle="target: #game-options-modal">
          Close
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import {
  Getter, State, Mutation, Action,
} from 'vuex-class';
import { Route } from 'vue-router';
import Velocity from 'velocity-animate';
import WaitingRoom from '@/components/WaitingRoom.vue';
import ChooseParty from '@/components/ChooseParty.vue';
import PlayerInfo from '@/components/PlayerInfo.vue';
import ApproveParty from '@/components/ApproveParty.vue';
import ApprovePartyResults from '@/components/ApprovePartyResults.vue';
import VoteQuest from '@/components/VoteQuest.vue';
import VoteQuestResults from '@/components/VoteQuestResults.vue';
import Lake from '@/components/Lake.vue';
import Assassinate from '@/components/Assassinate.vue';
import GameSummary from '@/components/GameSummary.vue';
import MidGameOptions from '@/components/MidGameOptions.vue';
import {
  IsDefaultStage,
  IsChoosePartyStage,
  IsApprovePartyStage,
  IsVoteQuestStage,
  IsLakeStage,
  IsAssassinateStage,
  IsEndStage,
  PartyLeader,
  IsConnectedToServer,
  IsConnectingToServer,
  IsHost,
} from '../store/getter-types';
import { QuestResult, Player } from '../types';
import { DisconnectFromServer, JoinGame } from '../store/action-types';
import { ClearGameState, SetUsername, SetPublicGameId } from '../store/mutation-types';
import { HomeRoute } from '../router/route-paths';

@Component({
  components: {
    WaitingRoom,
    ChooseParty,
    PlayerInfo,
    ApproveParty,
    ApprovePartyResults,
    VoteQuest,
    VoteQuestResults,
    MidGameOptions,
  },
})
export default class Play extends Vue {
  @Getter(IsDefaultStage) private isDefaultStage!: boolean;

  @Getter(IsChoosePartyStage) private isChoosePartyStage!: boolean;

  @Getter(IsApprovePartyStage) private isApprovePartyStage!: boolean;

  @Getter(IsVoteQuestStage) private isVoteQuestStage!: boolean;

  @Getter(IsLakeStage) private isLakeStage!: boolean;

  @Getter(IsAssassinateStage) private isAssassinateStage!: boolean;

  @Getter(IsEndStage) private isEndStage!: boolean;

  @Getter(PartyLeader) private partyLeader!: Player;

  @Getter(IsConnectedToServer) private isConnectedToServer!: boolean;

  @Getter(IsConnectingToServer) private isConnectingToServer!: boolean;

  @Getter(IsHost) private isHost!: boolean;

  @State private questResults!: QuestResult[];

  @State private questNumber!: number;

  @State private publicGameId!: string;

  @State private userApprovalVotes!: string;

  @State private questVotes!: string[];

  get haveUserApprovalVotes(): boolean {
    return Object.keys(this.userApprovalVotes).length !== 0;
  }

  get currentComponent() {
    if (this.isChoosePartyStage) {
      return ChooseParty;
    }
    if (this.isApprovePartyStage) {
      if (!this.haveUserApprovalVotes) {
        return ApproveParty;
      }
      return ApprovePartyResults;
    }
    if (this.isVoteQuestStage) {
      if (this.questVotes.length === 0) {
        return VoteQuest;
      }
      return VoteQuestResults;
    }
    if (this.isLakeStage) {
      return Lake;
    }
    if (this.isAssassinateStage) {
      return Assassinate;
    }
    if (this.isEndStage) {
      return GameSummary;
    }
    return { template: '<div></div>' };
  }

  private getClassForQuestResult(index: number) {
    if (this.questResults[index] === QuestResult.Unknown) {
      return '';
    }
    if (this.questResults[index] === QuestResult.GoodWins) {
      return 'uk-text-success uk-text-bold';
    }
    return 'uk-text-danger uk-text-bold';
  }

  private beforeEnter = (el: HTMLElement) => {
    // eslint-disable-next-line no-param-reassign
    el.style.opacity = '0';
  }

  private enter = (el: HTMLElement, done: () => void) => {
    Velocity(
      el,
      { opacity: 1 },
      { duration: 500, complete: done },
    );
  }

  private leave = (el: HTMLElement, done: () => void) => {
    Velocity(
      el,
      { opacity: 0 },
      { duration: 500, complete: done },
    );
  }

  @Action(JoinGame) private dispatchJoinGame!: () => Promise<void>;

  @Action(DisconnectFromServer) private disconnectFromServer!: () => void;

  @Mutation(ClearGameState) private clearGameState!: () => void;

  @Mutation(SetUsername) private setUsername!: (username: string) => void;

  @Mutation(SetPublicGameId) private setPublicGameId!: (id: string) => void;

  mounted() {
    if (!this.isConnectedToServer && !this.isConnectingToServer) {
      this.setUsername(this.$router.currentRoute.params.username);
      this.setPublicGameId(this.$router.currentRoute.params.publicGameId);
      this.dispatchJoinGame().catch(() => {
        this.$router.push(HomeRoute);
      });
    }
  }

  beforeRouteLeave(to: Route, from: Route, next: () => void) {
    this.clearGameState();
    next();
  }

  beforeDestroy() {
    this.disconnectFromServer();
  }
}
</script>

<style scoped>
#publicGameId {
  position: absolute;
  top: 80px;
  left: 5px;
}
</style>
