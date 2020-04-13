<template>
  <div class="uk-text-center">
    <h5 :class="textClassColor">{{playerDisplayText}}</h5>
    <div>
      <h5>All Players</h5>
      <div v-for="player in players" :key="player.username"
        class="uk-text-truncate uk-text-emphasis">
        <label class="uk-text-bold">
          {{player.isKing ? '(King)' : ''}}
          {{player.hasLake ? '(Lake)' : ''}}
        </label>
        <label>{{player.username}}</label>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Getter, State } from 'vuex-class';
import { Player, Alignment } from '@/types';
import { PlayerDisplayText, PlayerAlignment } from '../store/getter-types';

@Component
export default class PlayerInfo extends Vue {
  @Getter(PlayerDisplayText) private playerDisplayText!: string;

  @Getter(PlayerAlignment) private playerAlignment!: Alignment;

  @State private players!: Player[];

  get textClassColor() {
    if (this.playerAlignment === Alignment.Good) {
      return '';
    }
    return 'uk-text-danger';
  }
}
</script>
