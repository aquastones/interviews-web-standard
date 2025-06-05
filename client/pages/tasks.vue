<script setup lang="ts">
import { ref } from 'vue'

// Import components
import Toast from '~/components/Toast.vue'
import TaskForm from '~/components/TaskForm.vue'
import TaskList from '~/components/TaskList.vue'
import DeleteConfirm from '~/components/DeleteConfirm.vue'

// Output from useTasks.ts
const {
  tasks,
  toastMessage,
  showToast,
  showCreateForm,
  showEditForm,
  showDeleteConfirm,
  createInit,
  editInit,
  refreshTasks,
  handleCreate,
  handleEdit,
  startEdit,
  confirmDelete,
  deleteTask,
  toggleDone
} = useTasks()

// Fetch tasks on page load
await refreshTasks()
</script>

<template>
  <div class="min-h-screen bg-gray-950 text-white font-mono px-4 py-8">
    <!-- Toast Notification -->
    <Toast :message="toastMessage" v-model:show="showToast" />

    <div class="max-w-2xl mx-auto space-y-8">
      <!-- Header & Create Button -->
      <div class="flex justify-between items-center">
        <h1 class="text-4xl font-bold">
          Task Manager
        </h1>
        <button @click="showCreateForm = true" class="text-xl px-4 py-2 bg-sky-600 hover:bg-sky-300 rounded-xl">
          + New Task
        </button>
      </div>

      <!-- Task List Component -->
      <TaskList
        :tasks="tasks"
        @toggle-done="toggleDone"
        @edit="startEdit"
        @request-delete="confirmDelete"
      />

      <!-- Empty State Message -->
      <div v-if="!tasks.length" class="text-xl text-center text-gray-500 py-5">
        No tasks found.
      </div>

      <!-- Create Form -->
      <TaskForm
        mode="create"
        :visible="showCreateForm"
        :initialName="createInit.name"
        :initialDescription="createInit.description"
        :initialTags="createInit.tags"
        @submit="handleCreate"
        @cancel="showCreateForm = false"
        @error="(msg) => { toastMessage = msg; showToast = true }"
      />

      <!-- Edit Form -->
      <TaskForm
        mode="edit"
        :visible="showEditForm"
        :initialName="editInit.name"
        :initialDescription="editInit.description"
        :initialTags="editInit.tags"
        @submit="handleEdit"
        @cancel="showEditForm = false"
        @error="(msg) => { toastMessage = msg; showToast = true }"
      />

      <!-- Delete Confirmation Modal -->
      <DeleteConfirm
        :visible="showDeleteConfirm"
        @confirm="deleteTask"
        @cancel="showDeleteConfirm = false"
      />
    </div>
  </div>
</template>