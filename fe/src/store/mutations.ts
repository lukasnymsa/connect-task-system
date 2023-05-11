export default {
  setEmail(state: any, payload: any): void {
    state.email = payload.email;
  },
  setUser(state: any, payload: any): void {
    state.email = payload.email;
    state.token = payload.token;
  },
  unsetEmail(state: any): void {
    state.email = null;
  },
  unsetUser(state: any): void {
    state.email = null;
    state.token = null;
  },
  setError(state: any, payload: any): void {
    state.error = payload.error;
  },
  unsetError(state: any): void {
    state.error = null;
  }
}
