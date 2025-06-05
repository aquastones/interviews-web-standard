<script setup lang="ts">
import { ref } from 'vue'

// Import components
import Toast from '~/components/Toast.vue'
import TaskForm from '~/components/TaskForm.vue'
import TaskList from '~/components/TaskList.vue'
import DeleteConfirm from '~/components/DeleteConfirm.vue'

// Interfaces
interface Task
{
  id: number
  name: string
  description?: string
  done: boolean
  dateCreated: string
  tags: Tag[]
}
interface Tag
{
  id: number
  name: string
  color: string
}

// Variables
const tasks = ref<Task[]>([]) // Tasks array
const toastMessage = ref('') // Toast message

const createInit = { name: '', description: '', tags: '' } // Init values for a create form
const editInit = ref({ name: '', description: '', tags: '' }) // Init values for an edit form

const showToast = ref(false) // Toast visibility
const showCreateForm = ref(false) // Create form visibility
const showEditForm = ref(false) // Edit form visibility
const showDeleteConfirm = ref(false) // Delete confirmation visibility

const taskToEditId = ref<number | null>(null) // Edit task id
const taskToDeleteId = ref<number | null>(null) // Delete task id

// Functions
// Fetch tasks from API
const refreshTasks = async () =>
{
  try
  {
    const data = await $fetch<Task[]>('/api/tasks')
    tasks.value = data
  }
  catch (e)
  {
    console.error(e)
    toastMessage.value = 'Error loading tasks'
    showToast.value = true
  }
}

// Handle form submit for new task
const handleCreate = async (payload: { name: string, description: string, tags: string }) =>
{
  try
  {
    const newTask = await $fetch<Task>('/api/tasks', { method: 'POST', body: { name: payload.name, description: payload.description },}) // Create task on backend
    await $fetch(`/api/tasks/${newTask.id}/tags-multiple`, { method: 'POST', body: { tagString: payload.tags }, }) // Add tags
    showCreateForm.value = false // Hide the form
    await refreshTasks() // Refresh
  }
  catch (err)
  {
    console.error(err)
    toastMessage.value = 'Error creating task'
    showToast.value = true
  }
}

// Load task data into an edit form
const startEdit = (task: Task) =>
{
  taskToEditId.value = task.id
  editInit.value = { name: task.name, description: task.description || '', tags: task.tags.map((t) => t.name).join(' ') } // Load current data
  showEditForm.value = true // Show form
}

// Save edit
const handleEdit = async (payload: { name: string, description: string, tags: string }) =>
{
  if (!taskToEditId.value) return
  try
  {
    await $fetch(`/api/tasks/${taskToEditId.value}`, { method: 'PUT', body: { id: taskToEditId.value, name: payload.name, description: payload.description } }) // Save task
    await $fetch(`/api/tasks/${taskToEditId.value}/tags-multiple`, { method: 'POST', body: { tagString: payload.tags } }) // Add tags
    showEditForm.value = false // Hide form
    taskToEditId.value = null // Reset edit id
    await refreshTasks() // Refresh
  }
  catch (err)
  {
    console.error(err)
    toastMessage.value = 'Error updating task'
    showToast.value = true
  }
}

// Show confirmation pop up
const confirmDelete = (id: number) => { taskToDeleteId.value = id, showDeleteConfirm.value = true }

// Delete task from API
const deleteTask = async () =>
{
  if (!taskToDeleteId.value) return
  try
  {
    await $fetch(`/api/tasks/${taskToDeleteId.value}`, { method: 'DELETE' }) // Hit the delete endpoint
    showDeleteConfirm.value = false // Hide pop-up
    taskToDeleteId.value = null // Reset id
    await refreshTasks() // Refresh
  }
  catch (err)
  {
    console.error(err)
    toastMessage.value = 'Error deleting task'
    showToast.value = true
  }
}

// Toggle task completion
const toggleDone = async (id: number) =>
{
  try
  {
    await $fetch(`/api/tasks/${id}/done`, { method: 'PATCH' }) // Hit the toggle done endpoint
    await refreshTasks() // Refresh
  }
  catch (err)
  {
    console.error(err)
    toastMessage.value = 'Error marking task as done'
    showToast.value = true
  }
}

// Fetch tasks on page load
await refreshTasks()
</script>

<template>
  <div class="min-h-screen bg-gray-900 text-gray-100 font-mono px-4 py-8">
    <!-- Toast Notification -->
    <Toast :message="toastMessage" v-model:show="showToast" />

    <div class="max-w-2xl mx-auto space-y-8">
      <!-- Header & Create Button -->
      <div class="flex justify-between items-center">
        <h1 class="text-4xl font-bold">Task Manager</h1>
        <button
          @click="showCreateForm = true"
          class="text-xl px-4 py-2 bg-sky-600 hover:bg-sky-300 rounded"
        >
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
      <div v-if="!tasks.length" class="text-xl text-center text-gray-500">
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

<style>
/* Fade animation for transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>