<template>
    <aside class="md:w-1/4 border border-gray-700 bg-gray-800 p-3 rounded-xl">
        <h2 class="text-3xl font-bold mb-4">Tags</h2>
            <ul>
                <li v-for="tag in tags" :key="tag.id" class="flex justify-between items-center mb-2">
                    <button
                        @click="$emit('filter-tag', tag.id)"
                        class="flex-1 text-left px-2 py-1 rounded-xl text-sm hover:opacity-80"
                        :class="{ 'filter grayscale opacity-50': selectedTagIds.length && !selectedTagIds.includes(tag.id) }"
                        :style="{ backgroundColor: tag.color }">
                            {{ tag.name }}
                    </button>
                    <button
                        @click="$emit('delete-tag', tag.id)"
                        class="ml-2 text-red-500 hover:text-red-300">
                        -
                    </button>
                </li>
                <EmptyState
                    :visible="!tags.length"
                    message="No tags found."
                />
            </ul>
    </aside>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

interface Tag
{
  id: number
  name: string
  color: string
}

const props = defineProps<{
  tags: Tag[]
  selectedTagIds: number[]
}>()

const emit = defineEmits<{
  (e: 'filter-tag', tagId: number): void
  (e: 'delete-tag', tagId: number): void
}>()
</script>