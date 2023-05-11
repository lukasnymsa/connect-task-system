import {createStore} from "vuex";
import mutations from '@/store/mutations'
import actions from '@/store/actions'
import getters from '@/store/getters'


export interface State {
  email: string|null,
  token: string|null,
  error: string|null
}

const store = createStore<State>({
  state: {
    email: null,
    token: null,
    error: null
  },
  mutations: mutations,
  actions: actions,
  getters: getters
});

export default store;
