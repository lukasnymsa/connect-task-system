<script lang="ts" setup>
  import {computed, onMounted, ref, watchEffect} from "vue";
  import Bar from "@/components/Bar.vue";
  import LoginDialog from "@/dialogs/Auth.vue";
  import {useDisplay} from "vuetify";
  import store from "@/store/index"
  import router from "@/router";

  const loginDialog = ref<boolean>(false);
  const isMobile = computed<boolean>(() => useDisplay().mobile.value);

  const error = computed<string|null>(() => store.getters.error);

  watchEffect(() => {
    if (store.getters.isAuthenticated === false) {
      router.replace({name: 'Home'});
    }
  })

  onMounted(async () => {
    await store.dispatch('autoLogin');
  })
</script>

<template>
  <VApp>
    <VMain>
      <LoginDialog
        v-model="loginDialog"
        :width="isMobile === true ? '100%' : '60%'"
        :class="isMobile === true ? '' : 'rounded-xl pb-2'"
        @closeDialog="loginDialog = false"
      />

      <Bar
        @openDialog="loginDialog = true"
      />

      <VContainer
        class="justify-center pa-0 mx-auto ma-0 d-flex align-center text-center"
        :class="isMobile === true ? 'pt-0 pb-0' : 'pt-3 pb-3'"
      >
        <VSheet
          :width="isMobile === true ? '100%' : '60%'"
          class="pt-2"
          :class="isMobile === true ? '' : 'rounded-xl pr-10 pl-10 pb-2'"
          color="secondary_background"
        >
          <RouterView
            class="mx-auto pa-0 ma-0"
            @openDialog="loginDialog = true"
          />
        </VSheet>
        <VSnackbar
          v-model="error"
          multi-line
          color="error"
        >
          {{ error }}
          <template v-slot:actions>
            <VBtn
              color="white"
              variant="text"
              @click="error = null"
            >
              Close
            </VBtn>
          </template>
        </VSnackbar>
      </VContainer>
    </VMain>
  </VApp>
</template>
