<script lang="ts" setup>
  defineProps<{
    text: string,
    icon: string,
    color: string
  }>();
</script>

<template>
  <VChip
    class="ma-1 pa-3"
    :color="color"
    variant="elevated"
  >
    <VIcon class="pa-2">{{ icon }}</VIcon>
    <div class="pl-2">
      {{ text }}
    </div>
  </VChip>
</template>
