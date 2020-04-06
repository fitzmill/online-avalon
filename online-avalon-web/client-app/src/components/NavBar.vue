<template>
  <nav class="uk-navbar-container" uk-navbar>
    <div class="uk-navbar-left">
      <a class="uk-navbar-toggle" href="#nav-offcanvas" uk-toggle>
          <span uk-navbar-toggle-icon></span> <span class="uk-margin-small-left">Menu</span>
      </a>
    </div>
    <div class="uk-navbar-center">
      <a href="" class="uk-navbar-item uk-logo">Avalon</a>
    </div>
    <div id="nav-offcanvas" uk-offcanvas="overlay: true">
      <div class="uk-offcanvas-bar">

        <button class="uk-offcanvas-close" type="button" uk-close></button>

        <ul id="nav" class="uk-nav uk-nav-primary">
          <li><router-link to="/">Home</router-link></li>
          <li><router-link to="/about">About</router-link></li>
        </ul>

      </div>
    </div>
  </nav>
</template>

<style scoped>
#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
