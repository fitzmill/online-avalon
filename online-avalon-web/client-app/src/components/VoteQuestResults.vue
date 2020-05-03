<template>
  <div>
    <transition-group
      v-on:beforeEnter="beforeEnter"
      v-on:enter="enter"
      v-bind:css="false">
      <div v-for="(result, i) in computedQuestVotes" :key="i+0"
        :data-index="i">
        {{result}}
      </div>
    </transition-group>
    <transition
      v-on:beforeEnter="beforeEnter"
      v-on:enter="enter"
      v-bind:css="false">
      <h4 v-if="didFail === false" key="quest-pass">The quest has succeeded.</h4>
      <h4 v-else-if="didFail" key="quest-fail">The quest has failed.</h4>
    </transition>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import {
  State, Mutation, Action, Getter,
} from 'vuex-class';
import Velocity from 'velocity-animate';
import { SetCurrentQuestResult } from '../store/mutation-types';
import { QuestResult } from '../types';
import { SendEndQuestInfo } from '../store/action-types';
import { IsLeader } from '../store/getter-types';

@Component
export default class VoteQuestResults extends Vue {
  @Getter(IsLeader) private isKing!: boolean;

  @State private questVotes!: string[];

  private computedQuestVotes: string[] = [];

  @State private questResults!: QuestResult[];

  @State private questNumber!: number;

  private readonly listDelay = 750;

  private didFail: boolean | null = null;

  beforeEnter = (el: HTMLElement) => {
    // eslint-disable-next-line no-param-reassign
    el.style.opacity = '0';
  }

  enter(el: HTMLElement, done: () => void) {
    const index = el.dataset.index !== undefined
      ? Number(el.dataset.index) : this.questVotes.length;
    if (index === this.questVotes.length) {
      Velocity(
        el,
        { opacity: 1 },
        {
          delay: this.listDelay * index,
          duration: 1500,
          easing: 'ease-out',
          complete: () => {
            done();
            this.setCurrentQuestResult();
            if (this.isKing) {
              this.dispatchSendEndQuestInfo();
            }
          },
        },
      );
      return;
    }
    Velocity(
      el,
      { opacity: 1 },
      { delay: index * this.listDelay, complete: done },
    );
  }

  @Mutation(SetCurrentQuestResult) private setCurrentQuestResult!: () => void;

  @Action(SendEndQuestInfo) private dispatchSendEndQuestInfo!: () => Promise<void>;

  mounted() {
    this.computedQuestVotes = [...this.questVotes];
    this.didFail = this.questVotes.some((v) => v === 'Fail');
  }

  beforeDestroy() {
    this.didFail = null;
    this.computedQuestVotes = [];
  }
}
</script>
