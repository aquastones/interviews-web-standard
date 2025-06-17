<template>
  <li class="relative flex justify-between items-center mb-2">
    <button
      @click="$emit('filter-tag', tag.id)"
      class="flex-1 text-left px-2 py-1 rounded-xl text-sm hover:opacity-80"
      :class="{ 'filter grayscale opacity-50': selectedTagIds.length && !selectedTagIds.includes(tag.id) }"
      :style="{ backgroundColor: tag.color }">
      {{ tag.name }}

    </button>
    <!-- Buttons -->
    <button
      @click="open = !open"
      class="mx-1 px-2 py-1 rounded-xl bg-gray-700 hover:bg-sky-600 text-white text-sm">
      Edit
    </button>
    <div v-if="open" class="top-full absolute right-0 mt-1.5 bg-gray-800 border border-gray-700 rounded-xl space-y-1 p-1 z-10">
      <button
        @click="$emit('edit-tag', tag)"
        class="block w-full text-left px-2 py-1 rounded-xl text-xs hover:bg-sky-600">
        Change
      </button>
      <button
        @click="$emit('delete-tag', tag.id)"
        class="block w-full text-left px-2 py-1 rounded-xl text-xs text-red-600 hover:text-white hover:bg-red-600">
        Delete
      </button>
    </div>
  </li>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from 'vue'
import type { Tag } from '~/composables/useTasks'

// Input: Tag item & selected tags for filtering
const props = defineProps<{
  tag: Tag
  selectedTagIds: number[]
}>()

// Output: Button actions
const emit = defineEmits<{
  (e: 'filter-tag', tagId: number): void
  (e: 'edit-tag', tag: Tag): void
  (e: 'delete-tag', tagId: number): void
}>()

const open = ref(false)
</script>