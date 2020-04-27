<template>
    <div class="uk-flex uk-flex-column">
      <p class="uk-margin-top">Set the room name and enter your username</p>
      <input
        v-model="publicGameId"
        class="uk-input uk-margin-small-bottom"
        v-bind:class="{ 'uk-form-danger': errors.publicGameId }"
        type="text"
        placeholder="Create a room name" />
      <input
        v-model="username"
        class="uk-input uk-margin-bottom"
        v-bind:class="{ 'uk-form-danger': errors.username }"
        type="text"
        placeholder="Create a username" />

      <button class="uk-button uk-button-primary" @click="createGame()">Create Game</button>
    </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Action } from 'vuex-class';
import { SetUsername, SetPublicGameId } from '@/store/mutation-types';
import { CreateGame as CreateGameAction } from '@/store/action-types';
import { PlayRoute } from '@/router/route-paths';

@Component
export default class CreateGame extends Vue {
  @Action(CreateGameAction) dispatchCreateGame!: () => Promise<void>;

  get username() {
    return this.$store.state.username;
  }

  set username(value) {
    this.$store.commit(SetUsername, value);
    this.errors.username = false;
  }

  get publicGameId() {
    return this.$store.state.publicGameId;
  }

  set publicGameId(value) {
    this.$store.commit(SetPublicGameId, value);
    this.errors.publicGameId = false;
  }

  private errors: { [key: string]: boolean} = {};

  private async createGame() {
    const errors: { [key: string]: boolean } = {};
    let hasError = false;

    if (!this.username) {
      errors.username = true;
      hasError = true;
    }
    if (!this.publicGameId) {
      errors.publicGameId = true;
      hasError = true;
    }
    if (hasError) {
      this.errors = errors;
      return;
    }

    try {
      await this.dispatchCreateGame();
      this.$router.push({ path: `${PlayRoute}/${this.publicGameId}/${this.username}` });
    } catch (error) {
      // error handled in store code
    }
  }
}
</script>
