<template>
  <ul uk-accordion>
    <li>
      <a class="uk-accordion-title" href="#">Reset Game</a>
      <div class="uk-accordion-content uk-text-center">
        <div>Are you sure?</div>
        <button class="uk-button uk-button-small uk-button-danger"
          @click="resetGame"
          :disabled="loadingReset">Reset Game</button>
      </div>
    </li>
  </ul>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { Action } from 'vuex-class';
import { RestartGame } from '../store/action-types';

@Component
export default class MidGameOptions extends Vue {
  private loadingReset = false;

  @Action(RestartGame) private dispatchRestartGame!: () => Promise<void>;

  private async resetGame() {
    this.loadingReset = true;
    try {
      await this.dispatchRestartGame();
    } catch {
      // TODO handle in store code
    } finally {
      this.loadingReset = false;
    }
  }
}
</script>
