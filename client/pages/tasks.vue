<script setup lang="ts">
import { onMounted } from 'vue'
import { useTasks } from '~/composables/useTasks'

// Import components
import Toast from '~/components/Toast.vue'
import TaskForm from '~/components/TaskForm.vue'
import TaskList from '~/components/TaskList.vue'
import DeleteConfirm from '~/components/DeleteConfirm.vue'
import TagsSidebar from '~/components/TagsSidebar.vue'
import Header from '~/components/Header.vue'
import EmptyState from '~/components/EmptyState.vue'

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
      <!-- Sidebar for tags -->
      <TagsSidebar
        :tags="tags"
        :selectedTagIds="selectedTagIds"
        @filter-tag="toggleTagFilter"
        @delete-tag="confirmDeleteTag"
      />

      <main class="flex-1 space-y-8">
        <!-- Page header -->
        <Header
          title="Tasks"
          buttonLabel="+ New Task"
          @action="showCreateForm = true"
        />

        <!-- Task list -->
        <TaskList
          :tasks="filteredTasks"
          @toggle-done="toggleDone"
          @edit="startEdit"
          @request-delete="confirmDeleteTask"
        />

        <!-- Empty state -->
        <EmptyState
          :visible="!filteredTasks.length"
          message="No tasks found."
        />

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