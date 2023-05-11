export default {
  token(state: any): string|null {
    return state.token
  },
  email(state: any): string|null {
    return state.email
  },
  isAuthenticated(state: any): boolean {
    return !!state.token;
  },
  error(state: any): string|null {
    return state.error;
  }
}
