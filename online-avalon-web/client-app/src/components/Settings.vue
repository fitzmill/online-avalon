<template>
  <form class="uk-form-stacked">
    <div>
      <label class="uk-form-label">Theme</label>
      <div class="uk-form-controls">
        <select class="uk-select" v-model="selectedTheme">
          <option value="light">Light</option>
          <option value="dark">Dark</option>
        </select>
      </div>
    </div>
  </form>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import { State, Mutation } from 'vuex-class';
import { ThemeOption } from '../types';
import { SetTheme } from '../store/mutation-types';

@Component
export default class Settings extends Vue {
  @State private theme!: ThemeOption;

  @Mutation(SetTheme) private setTheme!: (theme: ThemeOption) => void;

  get selectedTheme() {
    return this.theme;
  }

  set selectedTheme(value: ThemeOption) {
    this.setTheme(value);
  }
}
</script>
