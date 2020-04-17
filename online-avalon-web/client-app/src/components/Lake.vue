<template>
  <div>
    <div v-if="hasLake">
      <div v-if="!lakedUserAlignment">
        <h4>Choose who to lake</h4>
        <div class="uk-child-width-1-2 uk-grid-small" uk-grid>
          <div v-for="username in usernamesToLake" :key="username">
            <button
              class="app-button uk-button-small uk-width-1-1 uk-text-truncate"
              :class="[username === lakedUsername ? 'uk-button-primary' : 'uk-button-default']"
              @click="setLakedUsername(username)"
              :disabled="loading">
              {{username}}
            </button>
          </div>
        </div>
        <button
          class="uk-button uk-button-success uk-margin-top"
          @click="lakePlayer()"
          :disabled="loading"
        >
          Submit
        </button>
      </div>
      <div v-else>
        <h4>{{lakedUsername}} is {{lakedUserAlignment}}</h4>
        <button
          class="uk-button uk-button-primary"
          @click="continueQuest()"
          :disabled="loading"
        >
          Continue
        </button>
      </div>
    </div>
    <div v-else>
      <div v-if="!lakedUsername">
        <h4>Waiting for {{playerWithLake.username}} to lake someone</h4>
      </div>
      <div v-else>
        <h4>{{lakedUsername}} has been laked!</h4>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import {
  State, Getter, Mutation, Action,
} from 'vuex-class';
import UIkit from 'uikit';
import { SetLakedUsername } from '@/store/mutation-types';
import { HasLake, PlayerWithLake } from '../store/getter-types';
import { LakePlayer, ContinueQuestAfterLake } from '../store/action-types';
import { Alignment, Player } from '../types';

@Component
export default class Lake extends Vue {
  @Getter(HasLake) private hasLake!: boolean;

  @Getter(PlayerWithLake) private playerWithLake!: Player;

  @State private usernamesToLake!: string[];

  @State private lakedUsername!: string;

  @State private lakedUserAlignment!: Alignment;

  private loading = false;

  @Mutation(SetLakedUsername) private setLakedUsername!: (username: string) => void;

  @Action(LakePlayer) private dispatchLakePlayer!: (username: string) => Promise<void>;

  @Action(ContinueQuestAfterLake) private dispatchContinueAfterLake!: () => Promise<void>;

  private async lakePlayer() {
    const username = this.lakedUsername;
    this.setLakedUsername(username);
    this.loading = true;
    try {
      await this.dispatchLakePlayer(username);
    } catch (error) {
      UIkit.notification(error.message, { status: 'danger' });
    } finally {
      this.loading = false;
    }
  }

  private async continueQuest() {
    this.loading = true;
    try {
      await this.dispatchContinueAfterLake();
    } catch (error) {
      UIkit.notification(error.message, { status: 'danger' });
    } finally {
      this.loading = false;
    }
  }
}
</script>
