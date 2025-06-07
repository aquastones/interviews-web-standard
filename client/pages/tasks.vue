<script setup lang="ts">
import { onMounted } from 'vue'
import { useTasks } from '~/composables/useTasks'

// Import components
import Toast from '~/components/Toast.vue'
import TaskForm from '~/components/TaskForm.vue'
import TaskList from '~/components/TaskList.vue'
import DeleteConfirm from '~/components/DeleteConfirm.vue'

const {
  tags,
  filteredTasks,
  toastMessage,
  showToast,
  showCreateForm,
  showEditForm,
  showDeleteConfirmTask,
  showDeleteConfirmTag,
  createInit,
  editInit,
  refreshTasks,
  refreshTags,
  handleCreate,
  handleEdit,
  startEdit,
  confirmDeleteTask,
  confirmDeleteTag,
  deleteTask,
  deleteTag,
  toggleDone,
  toggleTagFilter,
  selectedTagIds,
} = useTasks()

// load both tasks and tags on page load
onMounted(async () =>
{
  await Promise.all([refreshTasks(), refreshTags()])
})
</script>

<template>
  <div class="min-h-screen bg-gray-900 text-white font-mono px-4 py-8">
    <Toast :message="toastMessage" v-model:show="showToast" />

    <!-- 2-col container -->
    <div class="max-w-6xl mx-auto flex flex-col md:flex-row gap-8">

      <!-- Sidebar with tags -->
      <aside class="md:w-1/4 bg-gray-800 p-4 rounded-xl">
        <h2 class="text-2xl font-semibold mb-4">Tags</h2>
        <ul>
          <li v-for="tag in tags" :key="tag.id" class="flex justify-between items-center mb-2">
            <button
              @click="toggleTagFilter(tag.id)"
              class="flex-1 text-left px-2 py-1 rounded-xl text-sm hover:opacity-80"
              :class="{ 'filter grayscale opacity-50': selectedTagIds.length && !selectedTagIds.includes(tag.id) }"
              :style="{ backgroundColor: tag.color }">
              {{ tag.name }}
            </button>
            <button @click="confirmDeleteTag(tag.id)" class="ml-2 text-red-500 hover:text-red-300">
              -
            </button>
          </li>
          <li v-if="!tags.length" class="text-gray-500 text-sm">No tags found.</li>
        </ul>
      </aside>

      <main class="flex-1 space-y-8">
        <div class="flex justify-between items-center">
          <h1 class="text-4xl font-bold">Task Manager</h1>
          <button
            @click="showCreateForm = true"
            class="text-xl px-4 py-2 bg-sky-600 hover:bg-sky-500 rounded-xl">
            + New Task
          </button>
        </div>

        <TaskList
          :tasks="filteredTasks"
          @toggle-done="toggleDone"
          @edit="startEdit"
          @request-delete="confirmDeleteTask"
        />

        <div v-if="!filteredTasks.length" class="text-xl text-center text-gray-500 py-5">
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
          @error="msg => { toastMessage = msg; showToast = true }"
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
          @error="msg => { toastMessage = msg; showToast = true }"
        />

        <!-- Delete Confirmation for Tasks -->
        <DeleteConfirm
          :visible="showDeleteConfirmTask"
          mode="task"
          @confirm="deleteTask"
          @cancel="showDeleteConfirmTask = false"
        />

        <!-- Delete Confirmation for Tags -->
        <DeleteConfirm
          :visible="showDeleteConfirmTag"
          mode="tag"
          @confirm="deleteTag"
          @cancel="showDeleteConfirmTag = false"
        />
      </main>
    </div>
  </div>
</template>