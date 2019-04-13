<template>
  <Landing></Landing>
  <!--
  <div class="uk-container uk-container-expand">
    <nav class="uk-navbar-container" uk-navbar>
      <div class="uk-navbar-left">
        <div class="uk-navbar-item">
          <router-link to='/' class="uk-logo">Reddit Monitor</router-link>
        </div>
      </div>
      <div class="uk-navbar-right">
        <div class="uk-navbar-item">
          <router-link to='/admin' class="uk-button uk-button-default">Admin</router-link>
        </div>
      </div>
    </nav>
    <router-view></router-view>
  </div>-->
</template>

<script>
import Landing from './pages/Landing'


export default {
  name: "app",
  components: {
    Landing
  },
};
</script>

<style lang="scss">
// 1. Your custom variables and variable overwrites.

$global-primary-background: #f7b243;

// 2. Import default variables and available mixins.
@import "uikit/src/scss/variables-theme.scss";
@import "uikit/src/scss/mixins-theme.scss";

// 3. Your custom mixin overwrites.
@mixin hook-card() { color: #000; }

// 4. Import UIkit.
@import "uikit/src/scss/uikit-theme.scss";
</style>
