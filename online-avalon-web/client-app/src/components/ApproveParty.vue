<template>
  <div>
    <PartyNumber />
    <h4>Do you approve of this party?</h4>
    <div class="uk-child-width-1-2 uk-grid-small" uk-grid>
      <div v-for="player in partyMembers" :key="player.username">
        <div class="uk-text-truncate uk-text-emphasis">
          {{player.username}}
        </div>
      </div>
    </div>
    <div v-if="!voted" class="uk-child-width-1-2" uk-grid>
      <div><button
        class="uk-button uk-button-success uk-width-1-1"
        @click="voteForParty('Approve')"
        :disabled="loading">
        Approve
      </button></div>
      <div><button
        class="uk-button uk-button-danger uk-width-1-1"
        @click="voteForParty('Reject')"
        :disabled="loading">
        Reject
      </button></div>
    </div>
    <div v-else class="uk-margin-top">
      Your vote has been submitted.
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Player } from '@/types';
import { Getter, Action } from 'vuex-class';
import { PartyMembers } from '../store/getter-types';
import PartyNumber from './PartyNumber.vue';

@Component({
  components: {
    PartyNumber,
  },
})
export default class ApproveParty extends Vue {
  @Getter(PartyMembers) private partyMembers!: Player[];

  private loading = false;

  private voted = false;

  @Action private dispatchVoteForParty!: (vote: string) => Promise<void>

  private async voteForParty(vote: string) {
    this.loading = true;
    await this.dispatchVoteForParty(vote);
    this.voted = true;
    this.loading = false;
  }
}
</script>
