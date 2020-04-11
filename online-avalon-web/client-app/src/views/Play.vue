<template>
  <div>
    <WaitingRoom v-if="isDefaultStage" />
    <div v-else class="uk-position-center">
      <h3>Quest History</h3>
      <div class="uk-grid-divider uk-grid-medium uk-child-width-auto uk-flex-center" uk-grid>
        <div v-for="(result, i) in questResults" :key="i"
          :class="[i === questNumber ? 'uk-text-bold' : '']">
          {{i+1}}
          <div :class="[getClassForQuestResult(i)]">{{result}}</div>
        </div>
      </div>
      <div class="uk-card uk-card-default uk-width-xlarge@m">
        <div class="uk-card-header">
          <button class="uk-button uk-button-primary uk-button-small"
          >
            Player Info
          </button>
        </div>
        <div class="uk-card-body">
          <ChooseParty v-if="isChoosePartyStage" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Getter, State } from 'vuex-class';
import WaitingRoom from '@/components/WaitingRoom.vue';
import ChooseParty from '@/components/ChooseParty.vue';
import {
  IsDefaultStage,
  IsChoosePartyStage,
  IsApprovePartyStage,
  IsVoteQuestStage,
  IsLakeStage,
  IsAssassinateStage,
  IsEndStage,
} from '../store/getter-types';
import { QuestResult } from '../types';

@Component({
  components: {
    WaitingRoom,
    ChooseParty,
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

  @State private questResults!: QuestResult[];

  @State private questNumber!: number;

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
