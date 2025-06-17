<template>
  <aside class="md:w-1/4 border border-gray-700 bg-gray-800 p-3 rounded-xl">
    <h2 class="text-3xl font-bold mb-2">Tags</h2>
    <ul>
      <TagItem
        v-for="tag in tags"
        :key="tag.id"
        :tag="tag"
        :selected-tag-ids="selectedTagIds"
        @filter-tag="$emit('filter-tag', $event)"
        @edit-tag="$emit('edit-tag', $event)"
        @delete-tag="$emit('delete-tag', $event)"
      />

        <button
        @click="$emit('create-tag')"
        class="mb-1 w-52 text-left px-2 py-1 bg-gray-700 hover:bg-sky-600 text-white rounded-xl text-sm">
        + New Tag
        </button>

        <EmptyState
            :visible    ="!tags.length"
            message="No tags found."
        />
    </ul>
  </aside>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'
import type { Tag } from '~/composables/useTasks'

// Inputs: Tag list & selected tags for filtering
const props = defineProps<{
  tags: Tag[]
  selectedTagIds: number[]
}>()

// Outputs: Button functions
const emit = defineEmits<{
  (e: 'filter-tag', tagId: number): void
  (e: 'create-tag'): void
  (e: 'edit-tag', tag: Tag): void
  (e: 'delete-tag', tagId: number): void
}>()
</script>