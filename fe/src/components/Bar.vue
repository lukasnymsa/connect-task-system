<script lang="ts" setup>
  import {useTheme} from "vuetify";
  import Router from "@/router";
  import {computed} from "vue";
  import {useStore} from "vuex";
  import AuthMenu from "@/components/AuthMenu.vue";
  import router from "@/router";

  const emit = defineEmits<{
    (e: 'openDialog'): void
  }>();

  const store = useStore();
  const isAuthenticatedState = computed(() => store.getters.isAuthenticated);

  const theme = useTheme();
  const currentRouteName = computed(() => Router.currentRoute.value.name);

  const toggleTheme = () => {
    theme.global.name.value = theme.global.current.value.dark ? 'light' : 'dark';
  }

  const goBack = () => {
    Router.back()
  }

  const canGoBack = () => {
    return currentRouteName.value !== 'Home';
  }

  const userButtonAction = async () => {
    if (isAuthenticatedState.value) {
      try {
        await store.dispatch('logout');
        await router.replace({name: 'Home'});
      } catch (e: any) {
        console.log(e.message ?? 'Could not logout, try later.');
        return;
      }
    } else {
      emit('openDialog');
    }
  }
</script>

<template>
  <VAppBar elevation="0" class="shrink-on" color="bar">
    <VAppBarTitle>
      <VBtn v-if="canGoBack()" @click="goBack()" prepend-icon="mdi-arrow-left">BACK</VBtn>
    </VAppBarTitle>
    <VBtn icon="mdi-theme-light-dark" @click="toggleTheme" />
    <AuthMenu @openDialog="emit('openDialog')" @userAction="userButtonAction"/>
  </VAppBar>
</template>

