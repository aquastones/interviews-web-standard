<template>
  <div class="min-h-screen bg-gray-900 text-white font-mono p-8">
    <!-- Page Title -->
    <h1 class="text-5xl font-bold mb-8 text-center">Task List</h1>

    <!-- Task Creation Form -->
    <div class="mb-12 max-w-2xl mx-auto bg-gray-800 p-6 rounded-lg ">
      <h2 class="text-2xl font-bold mb-4">Create New Task</h2>

      <input v-model="name" type="text" placeholder="Name" class="w-full p-3 rounded bg-gray-700 text-white mb-4" />
      <textarea v-model="description" placeholder="Description"
        class="w-full p-3 rounded bg-gray-700 text-white mb-4"></textarea>
      <input v-model="newTagInput" @keydown.enter.prevent="addTag" type="text" placeholder="Add tags (press Enter)"
        class="w-full p-3 rounded bg-gray-700 text-white mb-4" />

      <div class="flex flex-wrap gap-2 mb-4">
        <span v-for="(tag, index) in newTags" :key="index"
          class="bg-sky-600 hover:bg-sky-300 px-3 py-1 rounded-full text-sm text-white transition-colors duration-300">
          {{ tag }}
          <button @click="removeTag(index)" class="ml-2 text-red-200 hover:text-red-400">x</button>
        </span>
      </div>

      <button @click="createTask"
        class="bg-sky-600 hover:bg-sky-300 text-white px-6 py-3 rounded-xl transition-colors duration-300">
        Create Task
      </button>
    </div>

    <!-- Task List -->
    <div v-if="pending" class="text-center text-gray-400">Loading tasks...</div>
    <div v-else-if="error" class="text-center text-3xl text-red-500">Failed to load tasks.</div>
    <div v-else>
      <ul class="space-y-6 max-w-4xl mx-auto">
        <li v-for="task in tasks" :key="task.id" class="bg-gray-800 p-6 rounded-lg transition relative">
          <!-- Right-aligned Control Buttons -->
          <div class="absolute top-4 right-4 flex flex-col items-end space-y-2">
            <button @click="deleteTask(task.id)"
              class="bg-red-600 hover:bg-red-400 text-white text-sm px-3 py-1 rounded-xl transition-colors duration-300">
              Delete
            </button>
            <button @click="startEdit(task)"
              class="bg-yellow-500 hover:bg-yellow-400 text-white text-sm px-3 py-1 rounded-xl transition-colors duration-300">
              Edit
            </button>
          </div>

          <!-- Mark as Done Button -->
          <div class="flex justify-start items-center mb-4">
            <button @click="toggleDone(task.id)"
              class="bg-green-600 hover:bg-green-400 text-white text-sm px-4 py-1 rounded-xl transition-colors duration-300">
              {{ doneTasks.has(task.id) ? 'Undo' : 'Done' }}
            </button>
          </div>

          <!-- Task Edit Form -->
          <div v-if="editingTaskId === task.id" class="mt-4 space-y-3">
            <input v-model="editName" class="w-full p-3 bg-gray-700 text-white rounded" />
            <textarea v-model="editDescription" class="w-full p-3 bg-gray-700 text-white rounded" />
            <input v-model="newTagInput"
              @keydown.enter.prevent="() => { if (newTagInput.trim()) { editTags.push(newTagInput.trim()); newTagInput = ''; } }"
              placeholder="Add tags" class="w-full p-3 bg-gray-700 text-white rounded" />
            <div class="flex flex-wrap gap-2">
              <span v-for="(tag, index) in editTags" :key="index"
                class="bg-sky-600 hover:bg-sky-300 text-white px-3 py-1 rounded-full text-sm transition-colors duration-300">
                {{ tag }}
                <button @click="editTags.splice(index, 1)" class="ml-2 text-red-200 hover:text-red-400">x</button>
              </span>
            </div>
            <div class="flex gap-3">
              <button @click="saveTask"
                class="bg-sky-600 hover:bg-sky-300 text-white px-4 py-2 rounded-xl transition-colors duration-300">
                Save
              </button>
              <button @click="cancelEdit"
                class="bg-red-600 hover:bg-red-300 text-white px-4 py-2 rounded-xl transition-colors duration-300">
                Cancel
              </button>
            </div>
          </div>

          <!-- Task Content -->
          <h2 :class="['text-3xl font-semibold mb-2', { 'line-through text-gray-500': doneTasks.has(task.id) }]">
            {{ task.name }}
          </h2>
          <p :class="{ 'line-through text-gray-500': doneTasks.has(task.id) }">
            {{ task.description || 'No description' }}
          </p>

          <!-- Tags -->
          <div v-if="task.tags?.length" class="mt-4">
            <ul class="flex flex-wrap gap-2 mt-1">
              <li v-for="tag in task.tags" :key="tag.id"
                class="bg-sky-600 hover:bg-sky-300 text-white px-3 py-1 rounded-full text-sm transition-colors duration-300">
                {{ tag.name }}
              </li>
            </ul>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

// ----------------------------
// Interfaces for typed data
// ----------------------------
interface Task {
  id: number;
  name: string;
  description?: string;
  tags?: Tag[]; // changed from taskTags
}

interface Tag {
  id: number;
  name: string;
}

// ----------------------------
// Reactive state
// ----------------------------
const name = ref('');
const description = ref('');
const newTagInput = ref('');
const newTags = ref<string[]>([]); // Tags input for task creation

const doneTasks = ref<Set<number>>(new Set()); // Track completed task IDs

// Editing form state
const editingTaskId = ref<number | null>(null);
const editName = ref('');
const editDescription = ref('');
const editTags = ref<string[]>([]); // Tags input for editing tasks

// ----------------------------
// Task completion toggling
// ----------------------------
const toggleDone = (id: number) => {
  doneTasks.value.has(id)
    ? doneTasks.value.delete(id)
    : doneTasks.value.add(id);
};

// ----------------------------
// New tag entry management
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
// Start editing a task
// ----------------------------
const startEdit = (task: Task) => {
  editingTaskId.value = task.id;
  editName.value = task.name;
  editDescription.value = task.description || '';
  editTags.value = task.tags?.map(tag => tag.name) || [];
};

// Cancel edit and reset form
const cancelEdit = () => {
  editingTaskId.value = null;
  editName.value = '';
  editDescription.value = '';
  editTags.value = [];
};

// ----------------------------
// Save updated task + tags
// ----------------------------
const saveTask = async () => {
  if (!editingTaskId.value) return;

  try {
    // Update task fields
    await $fetch(`/api/tasks/${editingTaskId.value}`, {
      method: 'PUT',
      body: {
        id: editingTaskId.value,
        name: editName.value,
        description: editDescription.value,
      },
    });

    // Create tags if needed and collect IDs
    const tagIds: number[] = [];
    for (const tagName of editTags.value) {
      const tag = await $fetch<Tag>('/api/tags', {
        method: 'POST',
        body: { name: tagName },
      });
      tagIds.push(tag.id);
    }

    // Clear all existing tags by overwriting with an empty array
    await $fetch(`/api/tasks/${editingTaskId.value}/tags`, {
      method: 'POST',
      body: [],
    });

    // Re-add updated tag IDs
    if (tagIds.length) {
      await $fetch(`/api/tasks/${editingTaskId.value}/tags`, {
        method: 'POST',
        body: tagIds,
      });
    }

    await refresh(); // Refresh task list
    cancelEdit(); // Exit edit mode
  } catch (err) {
    console.error('Update failed:', err);
    alert('Could not update task.');
  }
};

// ----------------------------
// Fetch tasks and tags on load
// ----------------------------
const { data: tasks, pending, error, refresh } = await useFetch<Task[]>('/api/tasks');
const { data: tags } = await useFetch<Tag[]>('/api/tags');

// ----------------------------
// Create new task with tags
// ----------------------------
const createTask = async () => {
  if (!name.value.trim()) return alert('Name is required.');

  try {
    // Create base task
    const newTask = await $fetch<Task>('/api/tasks', {
      method: 'POST',
      body: {
        name: name.value,
        description: description.value,
      },
    });

    // Create or reuse each tag
    const tagIds: number[] = [];
    for (const tagName of newTags.value) {
      const tag = await $fetch<Tag>('/api/tags', {
        method: 'POST',
        body: { name: tagName },
      });
      tagIds.push(tag.id);
    }

    // Link tags to task
    if (tagIds.length) {
      await $fetch(`/api/tasks/${newTask.id}/tags`, {
        method: 'POST',
        body: tagIds,
      });
    }

    // Reset form
    name.value = '';
    description.value = '';
    newTags.value = [];

    await refresh(); // Refresh list
  } catch (err) {
    console.error('Failed to create task:', err);
    alert('Error creating task');
  }
};

// ----------------------------
// Delete a task by ID
// ----------------------------
const deleteTask = async (id: number) => {
  try {
    await $fetch(`/api/tasks/${id}`, {
      method: 'DELETE',
    });
    await refresh(); // Update task list
  } catch (err) {
    console.error('Delete failed:', err);
    alert('Could not delete task.');
  }
};
</script>