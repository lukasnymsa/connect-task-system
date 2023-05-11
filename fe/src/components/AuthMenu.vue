<script lang="ts" setup>
  import {computed} from "vue";
  import store from "@/store/index";

  const isAuthenticatedState = computed(() => store.getters.isAuthenticated);

  const emit = defineEmits<{
    (e: 'openDialog'): void,
    (e: 'userAction'): void
  }>();
</script>

<template>
  <VMenu
    location="bottom"
    location-strategy="connected"
    :close-on-content-click="false"
    offset="8"
  >
    <template v-slot:activator="{ props }">
      <VBtn
        icon="mdi-account"
        :color="isAuthenticatedState ? 'button' : ''"
        v-bind="props"
        @click="!isAuthenticatedState && emit('openDialog')"
      />
    </template>
    <VList
      min-width="300"
      v-if="isAuthenticatedState"
      class="text-start"
    >
      <VListItem>
        <VCard
          prepend-icon="mdi-account"
          title="Logged User"
          :text="store.getters.email"
        >
          <VCardActions>
            <VBtn
              color="button"
              @click="emit('userAction')"
            >LOG OUT</VBtn>
          </VCardActions>
        </VCard>
      </VListItem>
    </VList>
  </VMenu>
</template>
