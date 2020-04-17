<template>
  <div>
    <h2>{{resultText}}</h2>
    <h4>Game Summary</h4>
    <div v-for="player in gameSummary.players" :key="player.username"
      class="uk-text-truncate uk-text-emphasis">
      ({{player.role}}) {{player.username}}
    </div>
    <button
      v-if="isHost"
      class="uk-button uk-button-primary uk-margin-top"
    >
      Start New Game
    </button>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State } from 'vuex-class';
import { GameSummary as GameSummaryType, GameResult } from '@/types';

@Component
export default class GameSummary extends Vue {
  @State private isHost!: boolean;

  @State private gameSummary!: GameSummaryType;

  get resultText() {
    return this.gameSummary.gameResult === GameResult.GoodWins
      ? 'Good Wins!' : 'Evil Wins!';
  }
}
</script>
