<template>
  <div>
    <h3>Waiting Room</h3>
    <p>Waiting for the host to start the game</p>
    <button
      class="uk-button uk-button-primary uk-button-small uk-margin-small-bottom"
      :uk-tooltip="isHost ? '' : 'Only the host can set options'"
      :disabled="!isHost"
      @click="showOptions = !showOptions"
    >Game Options</button>
    <div class="uk-text-bold uk-margin-small-bottom">Players</div>
    <div
      v-for="player in players"
      :key="player.username"
    >
      {{player.username}}
      <label class="uk-text-bold">{{player.username === hostUsername ? '(Host)' : ''}}</label>
    </div>
    <button
      class="uk-button uk-button-success uk-margin-top"
      @click="startGame()"
      :disabled="!isHost"
      :uk-tooltip="isHost ? '' : 'Only the host can start the game'">Start Game</button>

    <!-- Game Options Modal -->
    <transition name="game-options">
      <div v-if="showOptions"
        class="uk-card uk-card-default uk-card-body uk-position-center uk-flex uk-flex-column"
      >
        <button
          uk-close
          uk-tooltip="Close"
          class="uk-position-top-right uk-padding-small"
          @click="showOptions = !showOptions"
        ></button>
        <label class="uk-text-bold uk-margin-bottom">Game options</label>
        <label>Optional Roles</label>
        <ul class="uk-list uk-margin-small-top uk-text-center">
          <li><label>
            <input class="uk-checkbox" type="checkbox" v-model="optionalRoles.percival">
            Percival
          </label></li>
          <li><label>
            <input class="uk-checkbox" type="checkbox" v-model="optionalRoles.morgana">
            Morgana
          </label></li>
          <li><label>
            <input class="uk-checkbox" type="checkbox" v-model="optionalRoles.mordred">
            Mordred
          </label></li>
          <li><label>
            <input class="uk-checkbox" type="checkbox" v-model="optionalRoles.oberon"
              :disabled="oberonDisabled"
              :uk-tooltip="oberonDisabled ? 'More players required' : ''">
            Oberon
          </label></li>
        </ul>
        <button class="uk-button uk-button-primary" @click="saveOptions()">Save Options</button>
      </div>
    </transition>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State, Action, Getter } from 'vuex-class';
import { CreateGameOptions, Player } from '../types';
import { StartGame } from '../store/action-types';
import { IsHost } from '../store/getter-types';

@Component
export default class WaitingRoom extends Vue {
  @State private players!: Player[];

  @State private hostUsername!: string;

  @Getter(IsHost) private isHost!: boolean;

  @Action(StartGame) private dispatchStartGame!: (arg0: CreateGameOptions) => Promise<void>

  private showOptions = false;

  get oberonDisabled() {
    return this.players.length <= 5;
  }

  private readonly optionalRoles: { [key: string]: boolean } = {
    morgana: false,
    percival: false,
    oberon: false,
    mordred: false,
  }

  private gameOptions: CreateGameOptions = {
    optionalRoles: [],
  }

  private saveOptions() {
    this.gameOptions = {
      optionalRoles: Object.keys(this.optionalRoles)
        .filter((k) => this.optionalRoles[k]),
    };
    this.showOptions = false;
  }

  private async startGame() {
    try {
      await this.dispatchStartGame(this.gameOptions);
    } catch (error) {
      // error handled in store code
    }
  }
}
</script>

<style scoped>
.game-options-enter-active,
.game-options-leave-active {
  transition: all .5s ease;
}

.game-options-enter,
.game-options-leave-to {
  transform: translate(50px, -50%);
  opacity: 0;
}
</style>
