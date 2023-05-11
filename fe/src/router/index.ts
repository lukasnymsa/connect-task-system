import {createRouter, createWebHistory, NavigationGuardNext, RouteLocationNormalized} from 'vue-router'
import Home from "@/views/Home.vue"
import TaskList from "@/views/TaskList.vue";
import TaskDetail from "@/views/TaskDetail.vue";
import store from '@/store/index'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => Home,
    meta: { requiresUnauth: true }
  },
  {
    path: '/requests',
    name: 'TaskList',
    component: () => TaskList,
    meta: { requiresAuth: true },
    props: false
  },
  {
    path: '/requests/:id',
    name: 'TaskDetail',
    component: () => TaskDetail,
    meta: { requiresAuth: true },
    props: true
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
})

router.beforeEach((
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) => {
  if (to.meta.requiresAuth && !store.getters.isAuthenticated) {
    next({ name: 'Home' });
  } else if (to.meta.requiresUnauth && store.getters.isAuthenticated) {
    next({ name: 'TaskList' });
  } else {
    next();
  }
})

export default router
