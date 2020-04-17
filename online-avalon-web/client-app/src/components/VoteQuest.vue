<template>
  <div>
    <div v-if="isInParty">
      <div v-if="!voted">
        <h4>Decide the fate of the quest</h4>
        <div class="uk-flex uk-flex-column uk-align-center uk-width-medium@s">
          <button class="uk-button uk-button-success"
            @click="voteForQuest('Succeed')"
            :disabled="loading">
            Succeed
          </button>
          <button class="uk-button uk-button-danger uk-margin-top"
            @click="voteForQuest('Fail')"
            :disabled="loading">
            Fail
          </button>
        </div>
      </div>
      <div v-else>
        Your decision has been recorded, waiting on the rest of the party...
      </div>
    </div>
    <div v-else>
      Waiting for party members...
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Getter, State, Action } from 'vuex-class';
import { Player } from '@/types';
import { PartyMembers } from '../store/getter-types';
import { VoteForQuest } from '../store/action-types';

@Component
export default class VoteQuest extends Vue {
  @Getter(PartyMembers) private partyMembers!: Player[];

  @State private username!: string;

  get isInParty() {
    const index = this.partyMembers.findIndex((p) => p.username === this.username);
    return index !== -1;
  }

  private loading = false;

  private voted = false;

  @Action(VoteForQuest) private readonly dispatchVoteForQuest!: (vote: string) => Promise<void>

  private async voteForQuest(vote: string) {
    this.loading = true;
    await this.dispatchVoteForQuest(vote);
    this.voted = true;
    this.loading = false;
  }
}
</script>
