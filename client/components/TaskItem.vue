<template>
  <div class="border border-gray-700 rounded-lg p-4 space-y-2 bg-gray-800">
    <div class="flex justify-between items-center">
      <!-- Marked as done -->
      <h3 class="text-lg font-semibold flex items-center gap-2">
        {{ task.name }}
        <span v-if="task.done" class="text-green-400 text-sm font-normal">(done)</span>
      </h3>
      <!-- Buttons -->
      <div class="flex gap-2">
        <button @click="$emit('toggle-done', task.id)" class="px-2 py-1 rounded bg-sky-600 hover:bg-sky-500 text-white text-sm">
          Toggle Done
        </button>

        <button @click="$emit('edit', task)" class="px-2 py-1 rounded bg-sky-600 hover:bg-sky-500 text-white text-sm">
          Edit
        </button>

        <button @click="$emit('request-delete', task.id)" class="px-2 py-1 rounded bg-red-600 hover:bg-red-500 text-white text-sm">
          Delete
        </button>
      </div>
    </div>
    <p class="text-sm text-gray-400">{{ task.description }}</p>
    <TagList :tags="task.tags" />
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
interface Task
{
  id: number
  name: string
  description?: string
  done: boolean
  dateCreated: string
  tags: Tag[]
}

const props = defineProps<{task: Task}>()
</script>