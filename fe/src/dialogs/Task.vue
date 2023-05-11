<script lang="ts" setup>
  import {computed, ref} from "vue";
  import {useDisplay} from "vuetify";
  import {useStore} from "vuex";

  const store = useStore();

  const emit = defineEmits<{
    (e: 'closeDialog'): void
    (e: 'reloadTasks'): void
  }>();

  const subject = ref("");
  const subjectError = computed(() => validateNotEmpty(subject.value, 'Subject'))
  const description = ref("");
  const descriptionError = computed(() => validateNotEmpty(description.value, 'Description'))

  const formSubmitted = ref(false);

  const isMobile = computed(() => useDisplay().mobile.value);

  const validateNotEmpty = (value: string, name: string) => {
    if (value || !formSubmitted.value) return '';

    return name + ' cannot be empty.'
  }

  const sendForm = async () => {
    formSubmitted.value = true;

    await store.dispatch('createTask', {
      name: subject.value,
      description: description.value
    })

    formSubmitted.value = false;
    subject.value = '';
    description.value = '';
    emit('closeDialog');
    emit('reloadTasks');
  }
</script>

<template>
  <VDialog
    transition="dialog-top-transition"
    :width="isMobile === true ? '94%' : '60%'"
  >
    <VCard
      class="rounded-xl"
    >
      <VToolbar class="rounded-t-xl">
        <VToolbarTitle>
          New request
        </VToolbarTitle>
        <template v-slot:append>
          <VBtn @click="$emit('closeDialog')" icon="mdi-close-thick"></VBtn>
        </template>
      </VToolbar>
      <VCardText class="ma-3 pa-2">
        <VForm @submit.prevent>
          <VTextField
            label="Subject"
            v-model="subject"
            :error-messages="subjectError"
            class="rounded-t-xl"
          />
          <VTextarea
            label="Description"
            v-model="description"
            :error-messages="descriptionError"
            class="rounded-t-xl"
          />
          <div class="text-center">
            <VBtn
              size="large"
              type="submit"
              color="primary"
              @click="sendForm()"
            >
              Submit
            </VBtn>
          </div>
        </VForm>
      </VCardText>
    </VCard>
  </VDialog>
</template>
