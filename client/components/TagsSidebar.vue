<template>
    <aside class="md:w-1/4 border border-gray-700 bg-gray-800 p-3 rounded-xl">
        <!-- Title -->
        <h2 class="text-3xl font-bold mb-4">Tags</h2>
            <!-- New tag button -->
            <button
                @click="$emit('create-tag')"
                class="mb-4 w-full text-left px-2 py-1 bg-gray-700 hover:bg-sky-600 text-white rounded-xl text-sm">
                + New Tag
            </button>
            <!-- Item Container -->
            <ul>
                <li v-for="tag in tags" :key="tag.id" class="flex justify-between items-center mb-2">
                    <!-- Tag item (Filter button) -->
                    <button
                        @click="$emit('filter-tag', tag.id)"
                        class="flex-1 text-left px-2 py-1 rounded-xl text-sm hover:opacity-80"
                        :class="{ 'filter grayscale opacity-50': selectedTagIds.length && !selectedTagIds.includes(tag.id) }"
                        :style="{ backgroundColor: tag.color }">
                        {{ tag.name }}
                    </button>
                    <!-- Edit button -->
                    <button
                        @click="$emit('edit-tag', tag)"
                        class="text-left px-2 py-1 hover:text-sky-600 text-white rounded-xl text-sm"
                        title="Edit tag">
                        edit
                    </button>
                    <!-- Delete button -->
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
    (e: 'create-tag'): void
    (e: 'edit-tag', tag: Tag): void
    (e: 'delete-tag', tagId: number): void
}>()
</script>