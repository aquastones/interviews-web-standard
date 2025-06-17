<template>
  <div class="flex flex-wrap gap-2 text-sm">
    <span v-for="tag in tags" :key="tag.id" class="px-2.5 py-1 rounded-xl text-white" :style="{ backgroundColor: tag.color }">
      {{ tag.name }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'
import type { Task, Tag } from '~/composables/useTasks'

// Input: Tags list
const props = defineProps<{tags: Tag[]}>()
</script>