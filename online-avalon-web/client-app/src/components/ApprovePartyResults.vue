<template>
  <div>
    <PartyNumber />
    <transition-group
      v-on:before-enter="beforeEnter"
      v-on:enter="enter"
      v-bind:css="false">
      <div v-for="(vote, i) in staggeredApprovalVotes" :key="vote.username"
        :data-index="i">
        {{vote.username}}:
        <label :class="voteClassMap[vote.value]">
          {{vote.value}}
        </label>
      </div>
    </transition-group>
    <transition
      v-on:before-enter="beforeEnter"
      v-on:enter="enter"
      v-bind:css="false">
      <h4 v-if="didPartyFail" key="fail">Vote has failed.</h4>
      <h4 v-else-if="didPartyFail === false" key="pass">Vote has passed.</h4>
    </transition>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State, Mutation } from 'vuex-class';
import Velocity from 'velocity-animate';
import { ApprovalVoteOptions, QuestStage } from '../types';
import { SetQuestStage, IncrementPartyNumber, SetUserApprovalVotes } from '../store/mutation-types';
import PartyNumber from './PartyNumber.vue';

@Component({
  components: {
    PartyNumber,
  },
})
export default class ApprovePartyResults extends Vue {
  @State private userApprovalVotes!: { [key: string]: string };

  @State private partyNumber!: number;

  private voteClassMap = {
    Approve: 'uk-text-success',
    Reject: 'uk-text-danger',
  }

  private staggeredApprovalVotes: {}[] = [];

  private readonly listDelay = 750;

  private didPartyFail: boolean | null = null;

  setDidPartyFail() {
    const keys = Object.keys(this.userApprovalVotes);
    const numApprovalVotes = keys.filter(
      (k) => this.userApprovalVotes[k] === ApprovalVoteOptions.Approve,
    ).length;

    this.didPartyFail = numApprovalVotes < keys.length / 2;
  }

  beforeEnter = (el: HTMLElement) => {
    // eslint-disable-next-line no-param-reassign
    el.style.opacity = '0';
  }

  enter(el: HTMLElement, done: () => void) {
    const index = el.dataset.index !== undefined
      ? Number(el.dataset.index) : this.staggeredApprovalVotes.length;
    if (index === this.staggeredApprovalVotes.length) {
      Velocity(
        el,
        { opacity: 1 },
        {
          delay: this.listDelay * index,
          duration: 1500,
          easing: 'ease-out',
          complete: () => { done(); this.moveToNextQuestStage(); },
        },
      );
      return;
    }
    Velocity(
      el,
      { opacity: 1 },
      {
        delay: this.listDelay * index,
        complete: done,
      },
    );
  }

  moveToNextQuestStage() {
    if (this.didPartyFail) {
      if (this.partyNumber === 5) {
        this.setQuestStage(QuestStage.End);
      } else {
        this.incrementPartyNumber();
        this.setUserApprovalVotes({});
        this.setQuestStage(QuestStage.ChooseParty);
      }
    } else {
      this.setQuestStage(QuestStage.VoteQuest);
    }
  }

  @Mutation(SetQuestStage) private setQuestStage!: (stage: QuestStage) => void;

  @Mutation(IncrementPartyNumber) private incrementPartyNumber!: () => void;

  @Mutation(SetUserApprovalVotes)
  private setUserApprovalVotes!: (votes: { [key: string]: string }) => void;

  mounted() {
    this.setDidPartyFail();
    this.staggeredApprovalVotes = Object.keys(this.userApprovalVotes)
      .map((k) => ({
        username: k,
        value: this.userApprovalVotes[k],
      }));
  }

  beforeDestroy() {
    this.didPartyFail = null;
    this.staggeredApprovalVotes = [];
  }
}
</script>
