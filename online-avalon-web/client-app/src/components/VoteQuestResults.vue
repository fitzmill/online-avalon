<template>
  <div>
    <transition-group name="vote-quest-result-list">
      <div v-for="(result, i) in computedQuestVotes" :key="i+0">
        {{result}}
      </div>
    </transition-group>
    <transition name="vote-quest-result-list">
      <h4 v-if="didFail === false" key="quest-pass">The quest has succeeded.</h4>
      <h4 v-else-if="didFail" key="quest-fail">The quest has failed.</h4>
    </transition>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State, Mutation, Action } from 'vuex-class';
import { SetCurrentQuestResult } from '../store/mutation-types';
import { QuestResult } from '../types';
import { SendEndQuestInfo } from '../store/action-types';

@Component
export default class VoteQuestResults extends Vue {
  @State private questVotes!: string[];

  private computedQuestVotes: string[] = [];

  @State private questResults!: QuestResult[];

  @State private questNumber!: number;

  private readonly listDelay = 750;

  get didFail() {
    if (this.questResults[this.questNumber - 1] === QuestResult.Unknown) {
      return null;
    }
    if (this.questResults[this.questNumber - 1] === QuestResult.GoodWins) {
      return false;
    }
    return true;
  }

  @Mutation(SetCurrentQuestResult) private setCurrentQuestResult!: () => void;

  @Action(SendEndQuestInfo) private dispatchSendEndQuestInfo!: () => Promise<void>;

  mounted() {
    this.computedQuestVotes = [];

    this.questVotes.forEach((v, i) => {
      setTimeout(() => {
        this.computedQuestVotes.push(v);
      }, this.listDelay * i);
    });

    setTimeout(() => {
      this.setCurrentQuestResult();
      this.dispatchSendEndQuestInfo();
    }, this.listDelay * this.questVotes.length);
  }
}
</script>

<style>
.vote-quest-result-list-enter-active,
.vote-quest-result-list-leave-active {
  transition: all .5s ease;
}

.vote-quest-result-list-enter,
.vote-quest-result-list-leave-to {
  opacity: 0;
}
</style>
