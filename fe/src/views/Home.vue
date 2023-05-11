<script setup lang="ts">
  import UnauthenticatedCard from "@/components/UnauthenticatedCard.vue";

  defineEmits<{
    (e: 'openDialog'): void
  }>()
</script>

<template>
  <div class="pa-4">
    <div class="text-h3 text-center font-weight-bold">Request System</div>
    <VIcon class="mx-auto text-center" size="300">mdi-vuetify</VIcon>
    <VCard
      flat color="transparent"
      title="You can create new request here!"
    />
    <div>Please, log in to continue.</div>
    <UnauthenticatedCard @openDialog="$emit('openDialog')"></UnauthenticatedCard>
  </div>
</template>
