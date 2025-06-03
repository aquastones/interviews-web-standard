<script setup lang="ts">
import { ref } from 'vue';

// ----------------------------
// Interfaces
// ----------------------------
interface Task {
  id: number;
  name: string;
  description?: string;
  done: boolean;
  dateCreated: string;
  tags: Tag[];
}

interface Tag {
  id: number;
  name: string;
  color: string;
}

// ----------------------------
// State
// ----------------------------
const name = ref('');
const description = ref('');
const newTagInput = ref('');
const newTags = ref<string[]>([]);

const editingTaskId = ref<number | null>(null);
const editName = ref('');
const editDescription = ref('');
const editTags = ref<string[]>([]);

// ----------------------------
// Data fetching (Nuxt 3+)
// ----------------------------
const { data: tasks, pending, error, refresh } = await useAsyncData<Task[]>('tasks', () =>
  $fetch('/api/tasks')
);

const { data: tags } = await useAsyncData<Tag[]>('tags', () =>
  $fetch('/api/tags')
);

// ----------------------------
// Tag management
// ----------------------------
const addTag = () => {
  const tag = newTagInput.value.trim();
  if (tag && !newTags.value.includes(tag)) {
    newTags.value.push(tag);
  }
  newTagInput.value = '';
};

const removeTag = (index: number) => {
  newTags.value.splice(index, 1);
};

// ----------------------------
// Create task
// ----------------------------
const createTask = async () => {
  if (!name.value.trim()) return alert('Name is required.');

  try {
    const newTask = await $fetch<Task>('/api/tasks', {
      method: 'POST',
      body: {
        name: name.value,
        description: description.value,
      },
    });

    await $fetch(`/api/tasks/${newTask.id}/tags-multiple`, {
      method: 'POST',
      body: {
        tagString: newTags.value.join(' ')
      }
    });

    name.value = '';
    description.value = '';
    newTags.value = [];

    await refresh();
  } catch (err) {
    console.error('Failed to create task:', err);
    alert('Error creating task.');
  }
};

// ----------------------------
// Edit task
// ----------------------------
const startEdit = (task: Task) => {
  editingTaskId.value = task.id;
  editName.value = task.name;
  editDescription.value = task.description || '';
  editTags.value = task.tags.map(tag => tag.name);
};

const cancelEdit = () => {
  editingTaskId.value = null;
  editName.value = '';
  editDescription.value = '';
  editTags.value = [];
};

const saveTask = async () => {
  if (!editingTaskId.value) return;

  try {
    await $fetch(`/api/tasks/${editingTaskId.value}`, {
      method: 'PUT',
      body: {
        id: editingTaskId.value,
        name: editName.value,
        description: editDescription.value,
      },
    });

    await $fetch(`/api/tasks/${editingTaskId.value}/tags-multiple`, {
      method: 'POST',
      body: {
        tagString: editTags.value.join(' ')
      }
    });

    await refresh();
    cancelEdit();
  } catch (err) {
    console.error('Update failed:', err);
    alert('Could not update task.');
  }
};

// ----------------------------
// Delete task
// ----------------------------
const deleteTask = async (id: number) => {
  try {
    await $fetch(`/api/tasks/${id}`, {
      method: 'DELETE',
    });
    await refresh();
  } catch (err) {
    console.error('Delete failed:', err);
    alert('Could not delete task.');
  }
};

// ----------------------------
// Toggle task done status
// ----------------------------
const toggleDone = async (id: number) => {
  try {
    await $fetch(`/api/tasks/${id}/done`, {
      method: 'PATCH',
    });
    await refresh();
  } catch (err) {
    console.error('Failed to toggle task:', err);
    alert('Error marking task as done.');
  }
};
</script>

<template>
  <div class="min-h-screen bg-gray-900 text-gray-100 font-mono px-4 py-8">
    <div class="max-w-2xl mx-auto space-y-8">

      <!-- Header -->
      <h1 class="text-3xl font-bold text-center">Task Manager</h1>

      <!-- Create Task Form -->
      <div class="space-y-4 border border-gray-700 rounded-lg p-4">
        <h2 class="text-xl font-semibold">Create Task</h2>
        <input v-model="name" type="text" placeholder="Task name"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded outline-none focus:ring-2 focus:ring-sky-600" />
        <textarea v-model="description" placeholder="Description (optional)"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded outline-none focus:ring-2 focus:ring-sky-600"></textarea>

        <!-- Tag input -->
        <div class="flex gap-2 items-center">
          <input v-model="newTagInput" @keydown.enter.prevent="addTag" type="text" placeholder="Add tag"
            class="flex-1 bg-gray-800 border border-gray-700 p-2 rounded" />
          <button @click="addTag" class="px-3 py-1 rounded bg-sky-600 hover:bg-sky-300 text-black">Add</button>
        </div>

        <!-- Tag display -->
        <div class="flex flex-wrap gap-2">
          <span v-for="(tag, index) in newTags" :key="index"
            class="px-2 py-1 text-sm rounded bg-gray-700">
            {{ tag }}
            <button @click="removeTag(index)" class="ml-1 text-red-400">x</button>
          </span>
        </div>

        <!-- Submit -->
        <button @click="createTask" class="w-full py-2 mt-2 rounded bg-sky-600 hover:bg-sky-300 text-black">
          Create Task
        </button>
      </div>

      <!-- Task List -->
      <div v-if="tasks?.length" class="space-y-6">
        <div v-for="task in tasks" :key="task.id" class="border border-gray-700 rounded-lg p-4 space-y-2">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-semibold">
              {{ task.name }}
              <span v-if="task.done" class="text-green-400 ml-2">(done)</span>
            </h3>
            <div class="flex gap-2">
              <button @click="toggleDone(task.id)" class="px-2 py-1 rounded bg-sky-600 hover:bg-sky-300 text-black">
                Toggle Done
              </button>
              <button @click="startEdit(task)" class="px-2 py-1 rounded bg-sky-600 hover:bg-sky-300 text-black">
                Edit
              </button>
              <button @click="deleteTask(task.id)" class="px-2 py-1 rounded bg-red-600 hover:bg-red-400 text-black">
                Delete
              </button>
            </div>
          </div>
          <p class="text-sm text-gray-400">{{ task.description }}</p>
          <div class="flex flex-wrap gap-2 text-sm">
            <span v-for="tag in task.tags" :key="tag.id"
              class="px-2 py-1 rounded" :style="{ backgroundColor: tag.color }">
              {{ tag.name }}
            </span>
          </div>
        </div>
      </div>

      <!-- Edit Task Panel -->
      <div v-if="editingTaskId" class="space-y-4 border border-gray-700 rounded-lg p-4">
        <h2 class="text-xl font-semibold">Edit Task</h2>
        <input v-model="editName" type="text" placeholder="Task name"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded" />
        <textarea v-model="editDescription" placeholder="Description"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded"></textarea>
        <input v-model="editTags" type="text" placeholder="Tags (space-separated)"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded" />

        <div class="flex gap-2">
          <button @click="saveTask" class="flex-1 py-2 rounded bg-sky-600 hover:bg-sky-300 text-black">Save</button>
          <button @click="cancelEdit" class="flex-1 py-2 rounded bg-gray-700 hover:bg-gray-600">Cancel</button>
        </div>
      </div>
    </div>
  </div>
</template>