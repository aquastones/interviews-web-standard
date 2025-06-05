<template>
  <div v-if="visible" class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center">
    <div class="bg-gray-800 rounded-xl w-full max-w-sm p-6 relative">

      <!-- Pop-Up Text -->
      <h3 class="text-xl font-semibold mb-4 text-center">
        Are you sure?
      </h3>
      <p class="text-sm mb-6 text-center text-gray-300">
        This will permanently delete the task.
      </p>

      <!-- Buttons -->
      <div class="flex justify-around">
        <button @click="$emit('cancel')" class="px-4 py-2 bg-gray-600 hover:bg-gray-500 rounded-xl text-white">
          Cancel
        </button>

        <button @click="$emit('confirm')" class="px-4 py-2 bg-red-600 hover:bg-red-400 rounded-xl text-white">
          Yes, Delete
        </button>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'
const props = defineProps<{visible: boolean}>()
</script>