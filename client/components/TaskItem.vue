<template>
  <div class="border border-gray-700 rounded-xl p-4 space-y-2 bg-gray-800">
    <div class="flex justify-between items-center">
      <h3
        class="text-lg font-semibold flex items-center gap-2"
        :class="{ 'line-through text-gray-500': task.done }">
        {{ task.name }}
      </h3>

      <!-- Buttons -->
      <div class="relative">
        <button @click="showButtons = !showButtons" class="px-3 py-1 rounded-xl bg-gray-700 hover:bg-sky-600 text-white text-sm">
          Edit
        </button>
        <div v-if="showButtons" class="absolute right-0 mt-2 bg-gray-800 border border-gray-700 rounded-xl p-2 space-y-1 z-10">
          <button
            @click="$emit('toggle-done', task.id)"
            class="block w-full text-left px-2 py-1 rounded text-sm hover:bg-sky-600">
            {{ task.done ? 'Undo' : 'Done' }}
          </button>
          <button
            @click="$emit('edit', task)"
            class="block w-full text-left px-2 py-1 rounded text-sm hover:bg-sky-600">
            Change
          </button>
          <button
            @click="$emit('request-delete', task.id)"
            class="block w-full text-left px-2 py-1 rounded text-sm text-red-600 hover:text-white hover:bg-red-600">
            Delete
          </button>
        </div>
      </div>
    </div>
    <p class="text-sm text-gray-400" :class="{ 'line-through text-gray-500': task.done }" >{{ task.description }}</p>
    <TagList :tags="task.tags" />
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref } from 'vue'

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
const showButtons = ref(false)
</script>