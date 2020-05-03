<template>
  <div>
    <div v-if="isAssassin">
      <h4>Choose who to assassinate</h4>
      <div class="uk-child-width-1-2 uk-grid-small" uk-grid>
        <div v-for="username in usernamesToAssassinate" :key="username">
          <button
            class="app-button uk-button-small uk-width-1-1 uk-text-truncate"
            :class="[username === assassinatedUsername ? 'uk-button-primary' : 'uk-button-default']"
            @click="assassinatedUsername = username"
            :disabled="loading">
            {{username}}
          </button>
        </div>
      </div>
      <button
        class="uk-button uk-button-success uk-margin-top"
        @click="assassinate()"
        :disabled="loading"
      >
        Submit
      </button>
    </div>
    <div v-else>
      <h4>Waiting on the Assassin</h4>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Watch } from 'vue-property-decorator';
import { State, Action, Mutation } from 'vuex-class';
import { Role, QuestStage } from '../types';
import { AssassinatePlayer } from '../store/action-types';
import { MoveToNextQuestStage } from '../store/mutation-types';

@Component
export default class Assassinate extends Vue {
  @State private nextQuestStage!: QuestStage;

  @State private usernamesToAssassinate!: string[];

  @State private playerRole!: Role;

  get isAssassin() {
    return this.playerRole === Role.Assassin;
  }

  private assassinatedUsername = '';

  private loading = false;

  @Watch('nextQuestStage')
  onNextQuestStageChanged() {
    this.moveToNextQuestStage();
  }

  @Mutation(MoveToNextQuestStage) private moveToNextQuestStage!: () => void;

  @Action(AssassinatePlayer) private dispatchAssassinatePlayer!:
    (username: string) => Promise<void>;

  private async assassinate() {
    this.loading = true;
    try {
      await this.dispatchAssassinatePlayer(this.assassinatedUsername);
    } catch (error) {
      // error handled in store code
    } finally {
      this.loading = false;
    }
  }
}
</script>
