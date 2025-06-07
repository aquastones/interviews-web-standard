<template>
  <div class="space-y-6">
    <TaskItem
      v-for="task in tasks"
      :key="task.id"
      :task="task"
      @toggle-done="$emit('toggle-done', $event)"
      @edit="$emit('edit', $event)"
      @request-delete="$emit('request-delete', $event)"/>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'
import TaskItem from '~/components/TaskItem.vue'

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

const props = defineProps<{tasks: Task[]}>()

const emit = defineEmits<{
  (e: 'toggle-done', taskId: number): void
  (e: 'edit', task: Task): void
  (e: 'request-delete', taskId: number): void
}>()
</script>