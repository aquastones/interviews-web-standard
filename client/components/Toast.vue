<template>
  <!-- Fade transition -->
  <transition name="fade">
    <!-- Toast box (only visible when 'visible' is true) -->
    <div v-if="visible" class="fixed top-4 right-4 z-60 bg-red-500 text-white px-4 py-2 rounded shadow">
      {{ message }}
    </div>
  </transition>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

// Inputs: message to show & visibility switch
const props = defineProps<{ message: string; show: boolean }>()

// Outputs: 'update:show' to tell parent to hide the toast
const emit = defineEmits<{ (e: 'update:show', value: boolean): void }>()

// Local visibility state
const visible = ref(props.show)

// Watch the 'show' property for changes
watch
(
  () => props.show,
  (newVal) =>
  {
    visible.value = newVal

    // As soon as toast becomes visible, hide it after 3 seconds
    if (newVal)
    {
      setTimeout(() => emit('update:show', false), 3000)
    }
  }
)
</script>

<style>
/* Fade animation */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>