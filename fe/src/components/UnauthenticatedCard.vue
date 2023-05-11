<script lang="ts" setup>
  defineEmits<{
    (e: 'openDialog'): void
  }>();
</script>

<template>
  <VCardText
  >
    <VBtn
      prepend-icon="mdi-account"
      size="x-large"
      color="button"
      @click="$emit('openDialog')"
    >LOG IN</VBtn>
  </VCardText>
</template>
