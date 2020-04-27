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
import { State } from 'vuex-class';

@Component({
  components: {
    NavBar,
  },
})
export default class App extends Vue {
  @State private serverErrorMessage!: string;

  @State private serverMessage!: string;

  @Watch('serverErrorMessage')
  onServerError() {
    UIkit.notification(this.serverErrorMessage, { status: 'danger' });
  }

  @Watch('serverMessage')
  onServerMessage() {
    UIkit.notification(this.serverMessage);
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
}

.app-button {
  cursor: pointer;
}
</style>
