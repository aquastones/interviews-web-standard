<template>
  <div class="border border-gray-700 rounded-xl p-4 space-y-2 bg-gray-800">
    <div class="flex justify-between items-center">
      <h3
        class="text-lg font-semibold flex items-center gap-2"
        :class="{ 'line-through text-gray-500': task.done }">
        {{ task.name }}
      </h3>

      <!-- Buttons -->
      <div class="flex gap-2">
        <button @click="$emit('toggle-done', task.id)" class="px-2 py-1 rounded-xl bg-sky-600 hover:bg-sky-500 text-white text-sm">
          {{ task.done ? 'Undo' : 'Done' }}
        </button>

        <button @click="$emit('edit', task)" class="px-2 py-1 rounded-xl bg-sky-600 hover:bg-sky-500 text-white text-sm">
          Edit
        </button>

        <button @click="$emit('request-delete', task.id)" class="px-2 py-1 rounded-xl bg-red-600 hover:bg-red-400 text-white text-sm">
          Delete
        </button>
      </div>
    </div>
    <p class="text-sm text-gray-400" :class="{ 'line-through text-gray-500': task.done }" >{{ task.description }}</p>
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