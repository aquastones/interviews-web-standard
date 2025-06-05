<template>
  <!-- Form (only visible when 'visible' is true) -->
  <div v-if="visible" class="fixed inset-0 backdrop-blur z-50 flex items-center justify-center">
    <!-- Form content container -->
    <div class="border border-gray-800 bg-gray-900 rounded-xl w-full max-w-lg p-6 relative">
      <!-- Dynamic title based on the mode (Create or Edit) -->
      <h2 class="text-2xl font-semibold mb-4">
        {{ mode === 'create' ? 'Create Task' : 'Edit Task' }}
      </h2>

      <!-- Form -->
      <form @submit.prevent="onSubmit">
        <!-- Input field for task name, triggers error on empty input -->
        <input
          v-model="name"
          @input="nameError = false"
          type="text"
          placeholder="Task name (*)"
          :class="['w-full bg-gray-800 p-2 rounded-xl mb-3 focus:outline-none focus:ring-2',
            nameError
              ? 'border border-red-500'
              : 'border border-gray-700 focus:ring-sky-600'
          ]"/>

        <!-- Input field for task description -->
        <input
          v-model="description"
          type="text"
          placeholder="Description"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded-xl mb-3 focus:outline-none focus:ring-2 focus:ring-sky-600"/>

        <!-- Input field for task tags -->
        <input
          v-model="tags"
          type="text"
          placeholder="Tags"
          class="w-full bg-gray-800 border border-gray-700 p-2 rounded-xl mb-4 focus:outline-none focus:ring-2 focus:ring-sky-600"/>

        <!-- Buttons -->
        <div class="flex justify-end gap-2">
          <!-- Cancel button emits cancel event -->
          <button type="button" @click="emitCancel" class="px-4 py-2 bg-gray-700 hover:bg-gray-500 rounded-xl">
            Cancel
          </button>

          <!-- Submit button dynamically labeled based on mode -->
          <button type="submit" class="px-4 py-2 bg-sky-600 hover:bg-sky-300 rounded-xl">
            {{ mode === 'create' ? 'Create' : 'Save' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, defineProps, defineEmits } from 'vue'

// Inputs
interface Props
{
  mode: 'create' | 'edit'
  initialName?: string
  initialDescription?: string
  initialTags?: string
  visible: boolean
}
const props = defineProps<Props>()

// Outputs
const emit = defineEmits<{
  (e: 'submit', payload: { name: string; description: string; tags: string }): void
  (e: 'cancel'): void
  (e: 'error', message: string): void
}>()

// Reactive variables for form fields and error state
const name = ref(props.initialName || '')
const description = ref(props.initialDescription || '')
const tags = ref(props.initialTags || '')
const nameError = ref(false)

// Watch visibility to reset form state
watch
(
  () => props.visible,
  (newVal) =>
  {
    if (newVal)
    {
      name.value = props.initialName || ''
      description.value = props.initialDescription || ''
      tags.value = props.initialTags || ''
      nameError.value = false
    }
  }
)

// Validate the input before submitting
function onSubmit()
{
  if (!name.value.trim())
  {
    nameError.value = true
    emit('error', 'Task name is required')
    return
  }
  emit('submit',
  {
    name: name.value.trim(),
    description: description.value.trim(),
    tags: tags.value.trim(),
  })
}

// Cancel
function emitCancel()
{
  emit('cancel')
}
</script>