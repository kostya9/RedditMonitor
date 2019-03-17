import Vue from 'vue'
import App from './App.vue'
import VueRouter from 'vue-router';

import Home from "./pages/Home.vue";
import Admin from "./pages/Admin.vue";

const routes = [
  {path: '/', component: Home},
  {path: '/admin', component: Admin},
]

const router = new VueRouter({
  routes // short for `routes: routes`
});

Vue.use(VueRouter);

Vue.config.productionTip = false

new Vue({
  render: h => h(App),
  router
}).$mount('#app')
