import App from './App.vue'
import store from "@/store/index";

import { createApp } from 'vue'

import { registerPlugins } from '@/plugins'

const app = createApp(App)

registerPlugins(app)

app.use(store);

app.mount('#app')
