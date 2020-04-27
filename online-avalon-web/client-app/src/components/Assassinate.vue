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
import { Vue, Component } from 'vue-property-decorator';
import { State, Action } from 'vuex-class';
import { Role } from '../types';
import { AssassinatePlayer } from '../store/action-types';

@Component
export default class Assassinate extends Vue {
  @State private usernamesToAssassinate!: string[];

  @State private playerRole!: Role;

  get isAssassin() {
    return this.playerRole === Role.Assassin;
  }

  private assassinatedUsername = '';

  private loading = false;

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
