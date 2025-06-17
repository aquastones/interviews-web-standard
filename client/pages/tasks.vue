<script setup lang="ts">
import { onMounted } from 'vue'
import { useTasks } from '~/composables/useTasks'

// --------------------
// Import components
// --------------------

import Toast from '~/components/Toast.vue'
import TaskForm from '~/components/TaskForm.vue'
import TagForm from '~/components/TagForm.vue'
import TaskList from '~/components/TaskList.vue'
import DeleteConfirm from '~/components/DeleteConfirm.vue'
import TagsSidebar from '~/components/TagsSidebar.vue'
import Header from '~/components/Header.vue'
import EmptyState from '~/components/EmptyState.vue'

// ------------------------------
// Recieve data from components
// ------------------------------
const {
  tags,
  filteredTasks,
  toastMessage,
  showToast,
  showCreateTaskForm,
  showEditTaskForm,
  showCreateTagForm,
  showEditTagForm,
  showDeleteConfirmTask,
  showDeleteConfirmTag,
  createTaskInit,
  editTaskInit,
  createTagInit,
  editTagInit,
  refreshTasks,
  refreshTags,
  handleCreateTask,
  handleEditTask,
  startEditTask,
  startCreateTag,
  handleCreateTag,
  handleEditTag,
  startEditTag,
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
      <!-- Sidebar for tags -->
      <TagsSidebar
        :tags="tags"
        :selectedTagIds="selectedTagIds"
        @filter-tag="toggleTagFilter"
        @delete-tag="confirmDeleteTag"
        @create-tag="startCreateTag"
        @edit-tag="startEditTag"
      />

      <main class="flex-1 space-y-8">
        <!-- Page header -->
        <Header
          title="Tasks"
          buttonLabel="+ New Task"
          @action="showCreateTaskForm = true"
        />

        <!-- Task list -->
        <TaskList
          :tasks="filteredTasks"
          @toggle-done="toggleDone"
          @edit="startEditTask"
          @request-delete="confirmDeleteTask"
        />

        <!-- Empty state -->
        <EmptyState
          :visible="!filteredTasks.length"
          message="No tasks found."
        />

        <!-- Create Task Form -->
        <TaskForm
          mode="create"
          :visible="showCreateTaskForm"
          :initialName="createTaskInit.name"
          :initialDescription="createTaskInit.description"
          :initialTags="createTaskInit.tags"
          @submit="handleCreateTask"
          @cancel="showCreateTaskForm = false"
          @error="msg => { toastMessage = msg; showToast = true }"
        />

        <!-- Edit Task Form -->
        <TaskForm
          mode="edit"
          :visible="showEditTaskForm"
          :initialName="editTaskInit.name"
          :initialDescription="editTaskInit.description"
          :initialTags="editTaskInit.tags"
          @submit="handleEditTask"
          @cancel="showEditTaskForm = false"
          @error="msg => { toastMessage = msg; showToast = true }"
        />

        <!-- Delete Confirmation for Tasks -->
        <DeleteConfirm
          :visible="showDeleteConfirmTask"
          mode="task"
          @confirm="deleteTask"
          @cancel="showDeleteConfirmTask = false"
        />

        <!-- Create Tag Form -->
        <TagForm
          mode="create"
          :visible="showCreateTagForm"
          :initialName="createTagInit.name"
          @submit="handleCreateTag"
          @cancel="showCreateTagForm = false"
          @error="msg => { toastMessage = msg; showToast = true }"
        />

        <!-- Edit Tag Form -->
        <TagForm
          mode="edit"
          :visible="showEditTagForm"
          :initialName="editTagInit.name"
          @submit="handleEditTag"
          @cancel="showEditTagForm = false"
          @error="msg => { toastMessage = msg; showToast = true }"
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