<template>
  <div class="flex flex-wrap gap-2 text-sm">
    <span v-for="tag in tags" :key="tag.id" class="px-2 py-1 rounded text-white" :style="{ backgroundColor: tag.color }">
      {{ tag.name }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'
interface Tag
{
  id: number
  name: string
  color: string
}
const props = defineProps<{tags: Tag[]}>()
</script>