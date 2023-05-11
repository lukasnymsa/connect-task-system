<script lang="ts">
import {defineComponent} from "vue";

export default defineComponent({
  name: "TaskList",
  inheritAttrs: false
})
</script>

<script lang="ts" setup>
  import {computed, onMounted, ref} from "vue";
  import {useDisplay} from "vuetify";
  import {Task} from "@/models/Task";
  import TaskDialog from "@/dialogs/Task.vue";
  import CustomChip from "@/components/BaseChip.vue";
  import TaskStateAlert from "@/components/TaskStateBanner.vue";
  import {useStore} from "vuex";

  const store = useStore();

  const dialog = ref<boolean>(false);

  let taskList: Task[] = [];
  const isLoading = ref<boolean>(false);
  const isMobile = computed<boolean>(() => useDisplay().mobile.value);

  const loadTasks = async (): Promise<void> => {
    isLoading.value = true;
    taskList = await store.dispatch('getTasks');
    isLoading.value = false;
  }

  const truncate = (text: string, maxLength: number, clamp: string): string => {
    return text.length > maxLength
      ? text.slice(0, maxLength) + clamp
      : text;
  }

  onMounted(() => {
    loadTasks();
  })
</script>

<template>
  <div class="text-center text-h4">
    List of opened requests
    <VIcon icon="mdi-reload" @click="loadTasks()" />
  </div>
  <div class="text-center">
    <VBtn
      class="text-center"
      prepend-icon="mdi-plus"
      @click="dialog = true"
    >Create new request</VBtn>
  </div>
  <TaskDialog v-model="dialog" @closeDialog="dialog = false" @reloadTasks="loadTasks()"/>
  <div v-if="isLoading" class="text-center pa-5">
    <VProgressCircular
      color="secondary"
      indeterminate
      size="70"
    ></VProgressCircular>
  </div>
  <div v-else-if="!taskList.length" class="text-center">No requests found!</div>
  <VHover v-else
    v-for="(task, index) in taskList"
    :key="index"
    v-slot:default="{ isHovering, props }"
    open-delay="50"
  >
    <VCard
      :elevation="isHovering ? 16 : 2"
      :class="{ 'on-hover': isHovering, 'rounded-xl': !isMobile}"
      class="mx-auto ma-3 pa-2 text-start"
      v-bind="props"
    >
      <VCardTitle>
        {{ task.name }}
      </VCardTitle>
      <RouterLink
        style="text-decoration: none; color: inherit;"
        :to="{
          name: 'TaskDetail',
          params: {
            id: task.id
          }
        }"
      >
        <TaskStateAlert :state="task.state" />
      </RouterLink>
      <VDivider />
      <VCardText>
        {{ truncate(task.description, isMobile === true ? 80 : 160, '...') }}
      </VCardText>
      <VCardItem>
        <CustomChip
          color="light-blue-lighten-2"
          icon="mdi-calendar"
          :text="task.created.toLocaleString()"
        />
        <CustomChip
          color="indigo"
          icon="mdi-comment-multiple-outline"
          :text="task.commentCount.toString()"
        />
      </VCardItem>
      <VDivider />
      <VCardActions>
        <RouterLink
          :to="{
            name: 'TaskDetail',
            params: {
              id: task.id
            }
          }"
        >
          <VBtn>View</VBtn>
        </RouterLink>
      </VCardActions>
    </VCard>
  </VHover>
</template>
