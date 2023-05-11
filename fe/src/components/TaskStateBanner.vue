<script lang="ts" setup>
  import {TaskState} from "@/models/enums/TaskState";
  import {states} from '@/helpers/states'

  defineProps<{
    state: TaskState
  }>();
</script>

<template>
  <VCardText class="text-start pa-2">
    <VAlert
      :type="states[state].type"
      :title="state"
      :icon="states[state].icon"
      :text="states[state].notification"
      :color="states[state].color"
      elevation="2"
    ></VAlert>
  </VCardText>
</template>
