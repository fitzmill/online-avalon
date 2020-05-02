<template>
  <div uk-height-match>
    <div
      id="join-card"
      class="uk-card uk-card-default uk-card-body
       uk-position-center uk-margin-remove"
    >
      <div class="uk-flex uk-flex-column">
        <h2>Welcome!</h2>
        <p>Create a game or join one using the room name</p>
        <button
          class="uk-button uk-button-primary uk-margin-bottom"
          @click="createGame()">Create Game</button>
        <form v-on:submit.prevent="joinGame">
          <input
            v-model="publicGameId"
            class="uk-input"
            v-bind:class="{ 'uk-form-danger': errors.publicGameId }"
            type="text"
            @focus="$event.target.select()"
            placeholder="Enter room name" />
          <input
            v-model="username"
            class="uk-input uk-margin-small-top"
            v-bind:class="{ 'uk-form-danger': errors.username }"
            type="text"
            placeholder="Create a username" />
          <button
            class="uk-button uk-button-success uk-margin-top"
            @click="joinGame()"
            type="submit"
            :disabled="loading">Join Game</button>
        </form>
      </div>
    </div>
    <transition
      name="create-game-slide"
      v-on:before-enter="hideOverflow"
      v-on:after-enter="showOverflow"
      v-on:before-leave="hideOverflow"
      v-on:after-leave="showOverflow"
      >
      <div
        id="create-card"
        v-if="creatingGame"
        class="uk-card uk-card-default uk-card-body
        uk-width-large@m uk-position-center uk-margin-remove">
        <button class="uk-button uk-button-default" @click="createGame()">
          <span class="uk-margin-small-right" uk-icon="chevron-left"></span>
          Back
        </button>
        <CreateGame />
      </div>
    </transition>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Action } from 'vuex-class';
import CreateGame from '@/components/CreateGame.vue';
import { SetUsername, SetPublicGameId } from '@/store/mutation-types';
import { JoinGame } from '@/store/action-types';
import { PlayRoute } from '@/router/route-paths';

@Component({
  components: {
    CreateGame,
  },
})
export default class Home extends Vue {
  private creatingGame = false;

  private loading = false;

  @Action(JoinGame) dispatchJoinGame!: () => Promise<void>;

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

  private createGame() {
    this.creatingGame = !this.creatingGame;
  }

  private async joinGame() {
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

    this.loading = true;

    try {
      await this.dispatchJoinGame();
      this.$router.push({ path: `${PlayRoute}/${this.publicGameId}/${this.username}` });
    } catch {
      // error is handled inside of store code
    } finally {
      this.loading = false;
    }
  }

  // eslint-disable-next-line class-methods-use-this
  private hideOverflow() {
    document.body.classList.add('uk-overflow-hidden');
  }

  // eslint-disable-next-line class-methods-use-this
  private showOverflow() {
    document.body.classList.remove('uk-overflow-hidden');
  }
}
</script>

<style>
#join-card {
  z-index: 997;
}
#create-card {
  z-index: 998;
}
.create-game-slide-enter-active,
.create-game-slide-leave-active {
  transition: all .5s ease;
}

.create-game-slide-enter,
.create-game-slide-leave-to {
  transform: translate(100px, -50%);
  opacity: 0;
}
</style>
