import Vue from 'vue';
import VueRouter from 'vue-router';
import UIkit from 'uikit';
import { HomeRoute, PlayRoute, AboutRoute } from '@/router/route-paths';
import Home from '../views/Home.vue';

Vue.use(VueRouter);

const routes = [
  {
    path: HomeRoute,
    name: 'Home',
    component: Home,
  },
  {
    path: AboutRoute,
    name: 'About',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/About.vue'),
  },
  {
    path: PlayRoute,
    name: 'Play',
    component: () => import(/* webpackChunkName: "play" */ '@/views/Play.vue'),
  },
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

router.beforeEach((to, from, next) => {
  UIkit.offcanvas(document.getElementById('nav-offcanvas')).hide();
  next();
});

export default router;
