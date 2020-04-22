<template>
  <div>
    <PartyNumber />
    <transition-group name="user-approval-vote-list">
      <div v-for="vote in staggeredApprovalVotes" :key="vote.username">
        {{vote.username}}:
        <label :class="voteClassMap[vote.value]">
          {{vote.value}}
        </label>
      </div>
    </transition-group>
    <transition name="user-approval-vote-result">
      <h4 v-if="didPartyFail" key="fail">Vote has failed.</h4>
      <h4 v-else-if="didPartyFail === false" key="pass">Vote has passed.</h4>
    </transition>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State, Mutation } from 'vuex-class';
import { ApprovalVoteOptions, QuestStage } from '../types';
import { SetQuestStage, IncrementPartyNumber } from '../store/mutation-types';
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

    const failed = numApprovalVotes < keys.length / 2;

    setTimeout(() => {
      if (failed) {
        if (this.partyNumber === 5) {
          this.setQuestStage(QuestStage.End);
        } else {
          this.incrementPartyNumber();
          this.setQuestStage(QuestStage.ChooseParty);
        }
      } else {
        this.setQuestStage(QuestStage.VoteQuest);
      }
    }, 3000);

    this.didPartyFail = failed;
  }

  @Mutation(SetQuestStage) private setQuestStage!: (stage: QuestStage) => void;

  @Mutation(IncrementPartyNumber) private incrementPartyNumber!: () => void;

  mounted() {
    this.didPartyFail = null;
    this.staggeredApprovalVotes = [];
    const keys = Object.keys(this.userApprovalVotes);
    keys.forEach((k, i) => {
      const timeout = this.listDelay * i;
      setTimeout(() => {
        this.staggeredApprovalVotes.push({
          username: k,
          value: this.userApprovalVotes[k],
        });
      }, timeout);
    });
    setTimeout(() => {
      this.setDidPartyFail();
    }, this.listDelay * keys.length);
  }
}
</script>

<style>
.user-approval-vote-list-enter-active,
.user-approval-vote-list-leave-active {
  transition: all .5s;
}

.user-approval-vote-list-enter,
.user-approval-vote-list-leave-to {
  opacity: 0;
}

.user-approval-vote-result-enter-active,
.user-approval-vote-result-leave-active {
  transition: all .5s;
}

.user-approval-vote-result-enter,
.user-approval-vote-result-leave-to {
  opacity: 0;
}
</style>
