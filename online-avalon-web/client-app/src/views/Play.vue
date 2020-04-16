<template>
  <div>
    <label id="publicGameId">Lobby: {{publicGameId}}</label>
    <div class="uk-flex uk-flex-center uk-margin-top">
      <WaitingRoom v-if="isDefaultStage" />
      <div v-else>
        <h3>Quest History</h3>
        <div class="uk-grid-divider uk-grid-medium uk-child-width-auto uk-flex-center" uk-grid>
          <div v-for="(result, i) in questResults" :key="i"
            :class="[i === questNumber ? 'uk-text-bold' : '']">
            {{i+1}}
            <div :class="[getClassForQuestResult(i)]">{{result}}</div>
          </div>
        </div>
        <div class="uk-card uk-card-default uk-width-xlarge@s uk-margin-small-top">
          <div class="uk-card-header">
            <button class="uk-button uk-button-primary uk-button-small"
              uk-toggle="target: #player-info-modal">
              Player Info
            </button>
            <div v-if="partyLeader" class="uk-margin-small-top">
              {{partyLeader.username}} is the King
            </div>
          </div>
          <div class="uk-card-body">
            <transition name="play-component-fade" mode="out-in">
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
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Getter, State } from 'vuex-class';
import WaitingRoom from '@/components/WaitingRoom.vue';
import ChooseParty from '@/components/ChooseParty.vue';
import PlayerInfo from '@/components/PlayerInfo.vue';
import ApproveParty from '@/components/ApproveParty.vue';
import ApprovePartyResults from '@/components/ApprovePartyResults.vue';
import {
  IsDefaultStage,
  IsChoosePartyStage,
  IsApprovePartyStage,
  IsVoteQuestStage,
  IsLakeStage,
  IsAssassinateStage,
  IsEndStage,
  PartyLeader,
} from '../store/getter-types';
import { QuestResult, Player } from '../types';

@Component({
  components: {
    WaitingRoom,
    ChooseParty,
    PlayerInfo,
    ApproveParty,
    ApprovePartyResults,
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

  @State private questResults!: QuestResult[];

  @State private questNumber!: number;

  @State private publicGameId!: string;

  @State private userApprovalVotes!: string;

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
}
</script>

<style scoped>
#publicGameId {
  position: absolute;
  top: 80px;
  left: 5px;
}

.play-component-fade-enter-active,
.play-component-fade-leave-active {
  transition: opacity .3s ease;
}

.play-component-fade-enter,
.play-component-fade-leave-to {
  opacity: 0;
}
</style>
