<template>
  <nav class="uk-navbar-container" uk-navbar>
    <div class="uk-navbar-left">
      <a class="uk-navbar-toggle" href="#nav-offcanvas" uk-toggle>
          <span uk-navbar-toggle-icon></span> <span class="uk-margin-small-left">Menu</span>
      </a>
    </div>
    <div class="uk-navbar-center">
      <router-link to="/" class="uk-logo">Avalon</router-link>
    </div>
    <div id="nav-offcanvas" uk-offcanvas="overlay: true">
      <div class="uk-offcanvas-bar">

        <button class="uk-offcanvas-close" type="button" uk-close></button>

        <ul id="nav" class="uk-nav uk-nav-primary">
          <li><router-link to="/">Home</router-link></li>
          <li><router-link to="/about">About</router-link></li>
          <li uk-toggle="target: #settings-modal">
            <span uk-icon="cog"></span>
            Settings
          </li>
        </ul>

      </div>
    </div>

    <!-- Settings Modal -->
    <div id="settings-modal" uk-modal>
      <div class="uk-modal-dialog uk-modal-body">
        <Settings />
      </div>
    </div>
  </nav>
</template>

<script lang="ts">
import { Vue, Component } from 'vue-property-decorator';
import Settings from './Settings.vue';

@Component({
  components: {
    Settings,
  },
})
export default class NavBar extends Vue {
}
</script>

<style scoped>
#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
