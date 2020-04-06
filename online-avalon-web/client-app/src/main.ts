import Vue from 'vue';
import UIkit from 'uikit';
import Icons from 'uikit/dist/js/uikit-icons';
import 'uikit/dist/css/uikit.min.css';
import App from './App.vue';
import router from './router';
import store from './store';

Vue.config.productionTip = false;

UIkit.use(Icons);

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');
