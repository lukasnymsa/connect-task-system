<script lang="ts" setup>
  import {computed, ref} from "vue";
  import {useStore} from "vuex";
  import router from "@/router";

  const emit = defineEmits<{
    (e: 'closeDialog'): void
  }>();

  const store = useStore();
  const emailState = computed(() => store.getters.email);

  const emailFormInput = ref<string>("");
  const codeFormInput = ref<string>("");
  const error = ref<string>("");

  const isLoading = ref<boolean>(false);

  const changeEmail = () => {
    error.value = "";
    emailFormInput.value = "";
    codeFormInput.value = "";
    store.dispatch('unsetEmail');
  }

  const submitRequestCode = async () => {
    if (emailFormInput.value === '' || !emailFormInput.value.includes('@')) {
      error.value = 'Invalid email.';
      return;
    }

    try {
      isLoading.value = true;
      await store.dispatch('requestCode', {
        email: emailFormInput.value
      });
    } catch (e: any) {
      error.value = e.message || 'Could not send code, try later.';
      isLoading.value = false;
      emailFormInput.value = "";
      return;
    }

    error.value = "";
    isLoading.value = false;
  };

  const submitLogin = async () => {
    if (codeFormInput.value === '') {
      error.value = 'Code cannot be empty.';
      return;
    }

    try {
      isLoading.value = true;
      await store.dispatch('login', {
        email: emailFormInput.value,
        code: codeFormInput.value
      });
    } catch (e: any) {
      error.value = e.message || 'Could not login, try later.';
      isLoading.value = false;
      return;
    }

    error.value = "";
    isLoading.value = false;
    emailFormInput.value = "";
    codeFormInput.value = "";
    emit('closeDialog');
    await router.replace({name: 'TaskList'});
  };
</script>

<template>
  <VDialog
    transition="dialog-top-transition"
  >
    <VCard
      class="rounded-xl"
    >
      <VToolbar class="rounded-t-xl">
        <VToolbarTitle>
          Login
        </VToolbarTitle>
        <template v-slot:append>
          <VBtn @click="$emit('closeDialog')" icon="mdi-close-thick"></VBtn>
        </template>
      </VToolbar>
      <VCardText class="ma-3 pa-2 text-center">
        <div
          v-if="emailState"
          class="text-center pa-2"
        >
          Please provide code that has been sent to your email address <b>{{ emailState }}</b>.
          <br>
          <a href="" v-on:click.prevent @click="changeEmail()">
            Change email
          </a>
        </div>
        <VAlert
          v-if="error.length"
          class="mb-2 text-start"
          title="Error"
          :text="error"
          icon="mdi-alert-circle-outline"
          color="error"
        />
        <VProgressCircular
          v-if="isLoading"
          color="secondary"
          indeterminate
          size="70"
        ></VProgressCircular>
        <VForm @submit.prevent>
          <VTextField
            v-if="!emailState"
            label="Email"
            v-model="emailFormInput"
            v-on:keyup.enter="submitRequestCode()"
          />
          <VTextField
            v-else
            label="Code"
            v-model="codeFormInput"
            v-on:keyup.enter="submitLogin()"
          />
          <VBtn
            v-if="!emailState"
            block
            @click="submitRequestCode()"
          >Send</VBtn>
          <VBtn
            v-else
            block
            @click="submitLogin()"
          >Log In</VBtn>
        </VForm>
      </VCardText>
    </VCard>
  </VDialog>
</template>
