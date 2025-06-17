<template>
  <div v-if="visible" class="text-xl text-center text-gray-500 py-5">
    {{ message }}
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'

// Inputs: Message and visibility state
const props = defineProps<{
  message: string
  visible: boolean
}>()
</script>