import Vue from 'vue'
import App from './App.vue'
import VueRouter from 'vue-router';

import Home from "./pages/Home.vue";
import Admin from "./pages/Admin.vue";

import Vuikit from 'vuikit'
import VuikitIcons from '@vuikit/icons'
import AuthenticationStore from './authenticationStore.js';
import Axios from 'axios';
import BaseUrl from './BaseUrl';
import NotificationStore from './notificationStore';

//import '@vuikit/theme'


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

Axios.interceptors.request.use((c) => {
  c.baseURL = BaseUrl.Value;
  return c;
})

Axios.interceptors.request.use((c) => {
  c.headers['Authorization'] = `Bearer ${store.getToken()}`;
  return c;
})

Axios.interceptors.response.use(undefined, (err) => {
  if(err.status === 401) {
    store.signout();
  }

  throw err;
});

new Vue({
  render: h => h(App),
  router
}).$mount('#app')
