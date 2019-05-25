import Vue from 'vue'
import App from './App.vue'
import VueRouter from 'vue-router';

import Home from "./pages/Home.vue";
import Admin from "./pages/Admin.vue";

import Vuikit from 'vuikit'
import 'uikit';
import VuikitIcons from '@vuikit/icons'
import AuthenticationStore from './authenticationStore.js';
import Axios from 'axios';
import BaseUrl from './BaseUrl';
import NotificationStore from './notificationStore';

import AsyncStore from './asyncStore.js';

import SelectedSubredditsStore from './selectedSubredditsStore.js';

import "vue-virtual-scroller/dist/vue-virtual-scroller.css";
import VueVirtualScroller from 'vue-virtual-scroller'

Vue.use(VueVirtualScroller)


const routes = [
  {path: '/', component: Home},
  {path: '/admin', component: Admin},
]

const router = new VueRouter({
  routes // short for `routes: routes`
});


Vue.use(Vuikit)
Vue.use(VuikitIcons)
Vue.use(VueRouter);

Vue.config.productionTip = false

const store = new AuthenticationStore();
Vue.prototype.$auth = store;

const notifications = new NotificationStore();
Vue.prototype.$notifications = notifications;

const asyncStore = new AsyncStore();
Vue.prototype.$asyncStore = asyncStore;

const subredditStore = new SelectedSubredditsStore();
Vue.prototype.$subredditStore = subredditStore;

Axios.interceptors.request.use((c) => {
  asyncStore.sendCall();
  c.baseURL = BaseUrl.Value;
  return c;
});

Axios.interceptors.response.use((c) => {
  asyncStore.receiveCall();
  return c;
}, (e) => {
  asyncStore.receiveCall();
  return Promise.reject(e);
})

Axios.interceptors.request.use((c) => {
  c.headers['Authorization'] = `Bearer ${store.getToken()}`;
  return c;
})

Axios.interceptors.response.use(undefined, (err) => {
  if(err.response.status === 401) {
    store.signout();
    notifications.error('Your session has expired, please sign in')
  }
  else {
    return Promise.reject(err);
  }
});

new Vue({
  render: h => h(App),
  router
}).$mount('#app')
