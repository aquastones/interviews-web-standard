<template>
  <div class="flex justify-between items-center">
    <h1 class="text-4xl font-bold">{{ title }}</h1>
    <button
      @click="$emit('action')"
      class="text-xl px-4 py-2 bg-gray-700 hover:bg-sky-600 text-white rounded-2xl">
      {{ buttonLabel }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

// Inputs: Title and button label
const props = defineProps<{
  title: string
  buttonLabel: string
}>()

// Output: Button action
const emit = defineEmits<{
  (e: 'action'): void
}>()
</script>