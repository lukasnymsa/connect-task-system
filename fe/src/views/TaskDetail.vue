<script lang="ts">
export default {
  inheritAttrs: false,
}
</script>

<script lang="ts" setup>
  import {computed, onMounted, ref} from "vue";
  import {Task} from "@/models/Task";
  import {TaskState} from "@/models/enums/TaskState";
  import {useDisplay} from "vuetify";
  import TaskStateBanner from "@/components/TaskStateBanner.vue";
  import {useStore} from "vuex";

  const store = useStore();

  const props = defineProps<{
    id: string
  }>()

  let task = ref<Task | undefined>(undefined);
  const isLoading = ref(true);
  const isMobile = computed(() => useDisplay().mobile.value);
  const chatMaxHeight = computed(() => useDisplay().height.value * 0.5);

  const snackbar = ref<boolean>(false);
  const snackbarText = ref<string>();

  const message = ref("");
  const messageError = ref("");
  const messageSent = ref(false);
  const validateMessage = (value: string, approve: boolean|undefined) => {
    if (value || !messageSent.value || approve) return '';

    return 'Comment cannot be empty.'
  };

  const loadTask = async (forceLoad: boolean) => {
    if (task.value === undefined || forceLoad) {
      isLoading.value = true;
    }
    task.value = await store.dispatch('getTask', {id: props.id});
    if (task.value === undefined || forceLoad) {
      isLoading.value = false;
    }
  };

  const sendMessage = async (messageType: string) => {
    messageSent.value = true;
    messageError.value = validateMessage(message.value, messageType === 'approve' || messageType === 'retry')
    if (messageError.value.length !== 0) {
      return;
    }
    const data = await store.dispatch('doTaskAction', {
      id: props.id,
      messageType: messageType,
      message: message.value
    });
    messageSent.value = false;
    message.value = '';
    if (data !== null) {
      persistDataToTask(data);
    } else {
      await loadTask(false);
    }
    showSnackbar('Comment added successfully');
    await scrollToLastComment();
  };

  const showSnackbar = (text: string) => {
    snackbar.value = true;
    snackbarText.value = text;
  };

  const persistDataToTask = (data:any) => {
    task.value = {
      id: data.id,
      name: data.name,
      description: data.description,
      state: getTaskStateValueFromString(data.taskState),
      created: new Date(data.created),
      comments: data.comments
        ? data.comments.map((commentData: any) => {
            return {
              content: commentData.content,
              created: new Date(commentData.created)
            }
        })
        : null,
      commentCount: data.comments.length
    }
    isLoading.value = false;
  }

  const getTaskStateValueFromString = (taskStateString: keyof typeof TaskState): TaskState => {
    return TaskState[taskStateString];
  };

  const scrollToLastComment = async () => {
    document.getElementById('chat-scrollable')?.scrollTo({
      top: document.getElementById('chat-scrollable')?.scrollHeight,
      behavior: 'smooth',
    })
  };

  const scrollVisible = ref<boolean>(false);

  const scrollHandler = () => {
    scrollVisible.value = window.scrollY > 50;
  }

  const scrollTop = () => {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  };

  window.addEventListener('scroll', scrollHandler)


  onMounted(async () => {
    await loadTask(true);
    await scrollToLastComment()
  });
</script>

<template>
  <div v-if="isLoading" class="text-h4 text-transparent">Task</div>
  <div v-else class="text-h4" style="word-wrap: break-word">
    {{ task.name }}
    <VIcon icon="mdi-reload" @click="loadTask(true)" />
  </div>
  <div
    :class="isMobile === true ? '' : 'ma-2'"
  >
    <VProgressCircular
      v-if="isLoading"
      color="secondary"
      indeterminate
      size="70"
    ></VProgressCircular>

    <div v-else class="text-start">
      <TaskStateBanner :state="task.state"/>
      <VCard
        :subtitle="task.created.toLocaleString()"
        color="secondary_background"
        class="ma-2"
      >
        <VCardText style="white-space: pre-wrap">
          {{ task.description }}
        </VCardText>
      </VCard>
      <VDivider class="ma-2"></VDivider>
      <VSheet
        class="pa-2"
        :class="isMobile === true ? '' : 'rounded-xl'"
        color="secondary_background"
      >
        <VCard
          class="mx-auto mb-1"
          :class="[isLoading ? '' : 'overflow-y-auto', isMobile === true ? 'pa-0' : 'pa-1']"
          :max-height="chatMaxHeight"
          id="chat-scrollable"
        >
          <VList v-if="task?.comments?.length">
            <template
              v-for="(comment, index) in task.comments"
              :key="comment"
            >
              <VDivider inset v-if="index !== 0"></VDivider>

              <VListItem
                class="text-left d-flex justify-start align-start"
              >
                <VCardSubtitle>
                  {{ comment.created.toLocaleString() }}
                </VCardSubtitle>
                <VCardText style="white-space: pre-wrap">
                  {{ comment.content }}
                </VCardText>
              </VListItem>
            </template>
          </VList>
          <div v-else-if="!isLoading">
            <VCardText>No comments yet.</VCardText>
          </div>
        </VCard>
        <VForm
          v-if="!isLoading"
          validate-on="submit"
          @submit.prevent
        >
          <VTextarea
            bg-color="secondary_background"
            v-model="message"
            :error-messages="messageError"
            variant="outlined"
            label="New message"
            clear-icon="mdi-close-circle"
            clearable
            type="text"
            @click:clear="message = ''"
          />
          <VBtn
            v-if="task.state === TaskState.Retry"
            block
            class="pa-2"
            rounded="2"
            prepend-icon="mdi-send"
            @click="sendMessage('reopen')"
          >Send message and reopen</VBtn>
          <div v-else-if="task.state === TaskState.Processed">
            <VBtn
              block
              class="pa-2"
              rounded="2"
              color="teal-accent-3"
              prepend-icon="mdi-check-bold"
              @click="sendMessage('approve')"
            >Resolve</VBtn>
            <VSpacer></VSpacer>
            <VBtn
              block
              class="mt-2 pa-2"
              rounded="2"
              color="red-darken-4"
              prepend-icon="mdi-close-thick"
              @click="sendMessage('reject')"
            >Reject</VBtn>
          </div>
          <VBtn
            v-else
            block
            class="pa-2"
            rounded="2"
            prepend-icon="mdi-send"
            @click="sendMessage('comments')"
          >Send message</VBtn>
        </VForm>
      </VSheet>
    </div>
  </div>
  <!-- NOTIFICATION -->
  <VSnackbar
    v-model="snackbar"
    timeout="3000"
  >
    {{ snackbarText }}
    <template v-slot:actions>
      <VBtn
        variant="text"
        @click="snackbar = false"
      >
        Close
      </VBtn>
    </template>
  </VSnackbar>
  <VBtn
    v-if="isMobile && scrollVisible"
    class="fixed-bottom mb-1 mr-1"
    :style="{ position: 'fixed', right: '3vw', top: '20vw' }"
    icon="mdi-arrow-up-thick"
    color="primary"
    @click="scrollTop()"
  />
</template>

<style lang="sass">
  *
    text-decoration-color: white
</style>
