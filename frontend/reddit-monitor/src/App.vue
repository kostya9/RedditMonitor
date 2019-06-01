<template>
  <div>
      <vk-notification :messages.sync="messages" position="top-right"></vk-notification>
      <Landing v-if="!signedin"></Landing>
      <div class="uk-container uk-container-expand" v-else>
        <vk-navbar>
          <vk-navbar-nav slot="left">
            <vk-navbar-logo>
              <div>Reddit Monitor</div>
            </vk-navbar-logo>
            <vk-navbar-item>
              <router-link to='/' class="uk-button uk-button-default">Search</router-link>
            </vk-navbar-item>
            <vk-navbar-item>
              <router-link to='/top-images' class="uk-button uk-button-default">Top Images</router-link>
            </vk-navbar-item>
          </vk-navbar-nav>
          <vk-navbar-nav slot="right">
            <vk-navbar-item @click="fetchSubreddits()">
              <vk-button><vk-icons-cog></vk-icons-cog></vk-button>
            </vk-navbar-item>
            <vk-navbar-item>
              <vk-button-link href="#" class="uk-button uk-button-default" @click="signout()">Log out</vk-button-link>
            </vk-navbar-item>
          </vk-navbar-nav>
        </vk-navbar>
        <router-view></router-view>
      </div>
        <loading :active="showSpinner" 
        :can-cancel="false" 
        :is-full-page="true"></loading>
        <vk-modal :show.sync="show">
          <subreddit-selector :allSubreddits="subreddits"></subreddit-selector>
        </vk-modal>
  </div>
</template>

<script>
import axios from 'axios';
import SubredditSelector from './components/SubredditSelector.vue'
import Landing from './pages/Landing'
import Loading from 'vue-loading-overlay';
import 'vue-loading-overlay/dist/vue-loading.css';

export default {
  name: "app",
  data () {
    return {
      signedin: false,
      messages: [],
      showSpinner: false,
      subreddits: [],
      show: false
    }
  },
  created() {
    this.signedin = this.$auth.isSignedin();
    this.$auth.subscribeSignin(() => this.signedin = true);
    this.$auth.subscribeSignout(() => this.signedin = false);
    this.$notifications.onNewNotification((n) => this.messages.push(n));
    this.$asyncStore.onAsyncStateChanged((c) => 
    {
      this.showSpinner = c;
    });
  },
  components: {
    Landing, Loading, SubredditSelector
  },
  methods: {
    signout() {
      this.$auth.signout();
    },
    fetchSubreddits() {
      axios.get('api/subreddits')
            .then((r) => {
                this.subreddits = r.data;
                this.show = true;
            });
    }
  }
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
