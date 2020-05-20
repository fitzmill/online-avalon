<template>
  <div>
    <div v-if="gameSummary">
      <h2>{{resultText}}</h2>
      <h4>Game Summary</h4>
      <div v-for="player in gameSummary.players" :key="player.username"
        class="uk-text-truncate uk-text-emphasis">
        ({{player.role}}) {{player.username}}
      </div>
    </div>
    <div v-else>
      Couldn't fetch game summary
    </div>
    <button
      class="uk-button uk-button-primary uk-margin-top"
      :disabled="!isHost"
      @click="restartGame"
    >
      Start New Game
    </button>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State, Action, Getter } from 'vuex-class';
import { GameSummary as GameSummaryType, GameResult } from '@/types';
import { RestartGame } from '../store/action-types';
import { IsHost } from '../store/getter-types';

@Component
export default class GameSummary extends Vue {
  @Getter(IsHost) private isHost!: boolean;

  @State private gameSummary!: GameSummaryType;

  get resultText() {
    return this.gameSummary.gameResult === GameResult.GoodWins
      ? 'Good Wins!' : 'Evil Wins!';
  }

  @Action(RestartGame) private dispatchRestartGame!: () => Promise<void>;

  private async restartGame() {
    await this.dispatchRestartGame();
  }
}
</script>
