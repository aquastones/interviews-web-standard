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
        <button @click="open = !open" class="px-3 py-1 rounded-xl bg-gray-700 hover:bg-sky-600 text-white text-sm">
          Edit
        </button>
        <div v-if="open" class="top-full absolute right-0 mt-1.5 bg-gray-800 border border-gray-700 rounded-xl p-1 space-y-1 z-10">
          <button
            @click="$emit('toggle-done', task.id)"
            class="block w-full text-left px-2 py-1 rounded-xl text-sm hover:bg-sky-600">
            {{ task.done ? 'Undo' : 'Done' }}
          </button>
          <button
            @click="$emit('edit', task)"
            class="block w-full text-left px-2 py-1 rounded-xl text-sm hover:bg-sky-600">
            Change
          </button>
          <button
            @click="$emit('request-delete', task.id)"
            class="block w-full text-left px-2 py-1 rounded-xl text-sm text-red-600 hover:text-white hover:bg-red-600">
            Delete
          </button>
        </div>
      </div>
    </div>
    <p class="text-sm text-gray-400" :class="{ 'line-through text-gray-500': task.done }">
      {{ task.description }}
    </p>
    <p class="mb-3 text-sm text-gray-500" :class="{ 'line-through text-gray-500': task.done }">
      {{ task.dateCreated }}
    </p>
    <TagList :tags="task.tags" />
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref } from 'vue'
import type { Task } from '~/composables/useTasks'

// Button menu collapse flag
const open = ref(false)

// Input: task object
const props = defineProps<{task: Task}>()
</script>