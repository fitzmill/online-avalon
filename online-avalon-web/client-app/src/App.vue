<template>
  <div id="app">
    <NavBar />
    <router-view/>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Watch } from 'vue-property-decorator';
import UIkit from 'uikit';
import NavBar from '@/components/NavBar.vue';
import { State, Mutation } from 'vuex-class';
import { SetServerMessage, SetServerErrorMessage, SetTheme } from './store/mutation-types';
import { ThemeOption } from './types';

@Component({
  components: {
    NavBar,
  },
})
export default class App extends Vue {
  @State private serverErrorMessage!: string;

  @State private serverMessage!: string;

  @State private theme!: ThemeOption;

  @Mutation(SetServerMessage) private setServerMessage!: (message: string) => void;

  @Mutation(SetServerErrorMessage) private setServerErrorMessage!: (message: string) => void;

  @Mutation(SetTheme) private setTheme!: (theme: ThemeOption) => void;

  @Watch('serverErrorMessage')
  onServerError() {
    if (this.serverErrorMessage && this.serverErrorMessage !== '') {
      UIkit.notification(this.serverErrorMessage, { status: 'danger' });
    }
  }

  @Watch('serverMessage')
  onServerMessage() {
    if (this.serverMessage && this.serverMessage !== '') {
      UIkit.notification(this.serverMessage);
    }
  }

  @Watch('theme')
  onThemeChange() {
    if (this.theme === 'light') {
      const link = document.getElementById('dark-mode-css');
      if (link) {
        document.head.removeChild(link);
      }
    } else if (this.theme === 'dark') {
      const link = document.createElement('link');
      link.id = 'dark-mode-css';
      link.type = 'text/css';
      link.rel = 'stylesheet';
      link.href = `${process.env.BASE_URL}dark-mode.css`;
      document.head.appendChild(link);
    }
  }

  resetMessages() {
    this.setServerMessage('');
    this.setServerErrorMessage('');
  }

  mounted() {
    UIkit.util.on(document, 'close', this.resetMessages);
    const cachedTheme = window.localStorage.getItem('theme') as ThemeOption;
    if (cachedTheme) {
      this.setTheme(cachedTheme);
    }
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  min-height: 100vh;
}

.uk-button-success {
  background-color: #1a921a;
  color: white;
  border: 1px solid transparent;
}

.uk-button-success:disabled {
  background-color: transparent;
  color: #999;
  border-color: #e5e5e5;
}

.app-button {
  cursor: pointer;
}
</style>
