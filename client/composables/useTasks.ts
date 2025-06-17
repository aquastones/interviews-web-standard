import { ref, computed, onMounted } from 'vue'

// -----------------
// Interfaces
// -----------------

export interface Task
{
    id: number
    name: string
    description?: string
    done: boolean
    dateCreated: string
    tags: Tag[]
}
export interface Tag
{
    id: number
    name: string
    color: string
}

export function useTasks()
{
    // -----------------
    // Variables
    // -----------------

    const tasks = ref<Task[]>([]) // Tasks array
    const tags = ref<Tag[]>([]) // Tags array

    const toastMessage = ref('') // Toast message

    const createTaskInit = { name: '', description: '', tags: '' } // Init values for a task create form
    const editTaskInit = ref({ name: '', description: '', tags: '' }) // Init values for a task edit form
    const createTagInit = { name: '' } // Init values for a tag create form
    const editTagInit = ref({ name: '' }) // Init values for a tag edit form

    const showToast = ref(false) // Toast visibility

    const showCreateTaskForm = ref(false) // Create task form visibility
    const showEditTaskForm = ref(false) // Edit task form visibility
    const showCreateTagForm = ref(false) // Create tag form visibility
    const showEditTagForm = ref(false) // Edit tag form visibility

    const showDeleteConfirmTask = ref(false) // Delete confirmation visibility for tasks
    const showDeleteConfirmTag = ref(false) // Delete confirmation visibility for tags

    const taskToEditId = ref<number | null>(null) // Edit task id
    const taskToDeleteId = ref<number | null>(null) // Delete task id
    const tagToEditId = ref<number | null>(null) // Edit tag id
    const tagToDeleteId = ref<number | null>(null) // Delete tag id

    const selectedTagIds = ref<number[]>([]) // Tag ids for filtering

    // -----------------
    // Functions
    // -----------------

    // Toggle a tag as an active filter
    const toggleTagFilter = (tagId: number) =>
    {
        const idx = selectedTagIds.value.indexOf(tagId)
        if (idx === -1) selectedTagIds.value.push(tagId)
        else selectedTagIds.value.splice(idx, 1)
    }

    // Tasks filtered by the selected tags
    const filteredTasks = computed(() =>
    {
        if (!selectedTagIds.value.length) return tasks.value
        return tasks.value.filter(task =>
            selectedTagIds.value.every(tagId =>
                task.tags.some(t => t.id === tagId)))
    })

    // Fetch tasks from API
    const refreshTasks = async () =>
    {
        try
        {
            tasks.value = await $fetch<Task[]>('/api/tasks')
        }
        catch (e)
        {
            toastMessage.value = 'Error loading tasks'
            showToast.value = true
        }
    }

    // Fetch tags from API
    const refreshTags = async () =>
    {
        try
        {
            tags.value = await $fetch<Tag[]>('/api/tags')
        }
        catch (e)
        {
            toastMessage.value = 'Error loading tags'
            showToast.value = true
        }
    }

    // Create task on the backend
    const handleCreateTask = async (payload: { name: string; description: string; tags: string }) => {
        try
        {
            const newTask = await $fetch<Task>('/api/tasks', {method: 'POST',body: { name: payload.name, description: payload.description },})
            await $fetch(`/api/tasks/${newTask.id}/tags-multiple`, {method: 'POST',body: { tagString: payload.tags },})
            showCreateTaskForm.value = false
            await refreshTags()
            await refreshTasks()
        }
        catch (err)
        {
            toastMessage.value = 'Error creating task'
            showToast.value = true
        }
    }

    // Init edit task
    const startEditTask = (task: Task) =>
    {
        taskToEditId.value = task.id
        editTaskInit.value =
        {
            name: task.name,
            description: task.description || '',
            tags: task.tags.map(t => t.name).join(' '),
        }
        showEditTaskForm.value = true
    }

    // Save task edit changes
    const handleEditTask = async (payload: { name: string; description: string; tags: string }) =>
    {
        if (!taskToEditId.value) return
        try
        {
            await $fetch(`/api/tasks/${taskToEditId.value}`,{method: 'PUT', body: {id: taskToEditId.value, name: payload.name, description: payload.description},})
            await $fetch(`/api/tasks/${taskToEditId.value}/tags-multiple`, {method: 'POST', body: {tagString: payload.tags},})
            showEditTaskForm.value = false
            taskToEditId.value = null
            await refreshTags()
            await refreshTasks()
        }
        catch (err)
        {
            toastMessage.value = 'Error updating task'
            showToast.value = true
        }
    }

    // Start task delete dialog
    const confirmDeleteTask = (id: number) =>
    {
        taskToDeleteId.value = id
        showDeleteConfirmTask.value = true
    }

    // Delete task
    const deleteTask = async () =>
    {
        if (!taskToDeleteId.value) return
        try
        {
            await $fetch(`/api/tasks/${taskToDeleteId.value}`, { method: 'DELETE' })
            showDeleteConfirmTask.value = false
            taskToDeleteId.value = null
            await refreshTags()
            await refreshTasks()
        }
        catch (err)
        {
            toastMessage.value = 'Error deleting task'
            showToast.value = true
        }
    }

    // Start creating a tag
    const startCreateTag = () =>
    {
      createTagInit.name = ''
      showCreateTagForm.value = true
    }

    // Create a new tag
    const handleCreateTag = async (payload: { name: string }) =>
    {
      try
      {
        await $fetch<Tag>('/api/tags', {method: 'POST', body: { name: payload.name },})
        showCreateTagForm.value = false
        await Promise.all([refreshTasks(), refreshTags()])
      }
      catch (err)
      {
        toastMessage.value = 'Error creating tag'
        showToast.value = true
      }
    }

    // Start editing a tag
    const startEditTag = (tag: Tag) =>
    {
      tagToEditId.value = tag.id
      editTagInit.value = { name: tag.name }
      showEditTagForm.value = true
    }

    // Save tag edits
    const handleEditTag = async (payload: { name: string }) =>
    {
      if (!tagToEditId.value) return
      try
      {
        await $fetch(`/api/tags/${tagToEditId.value}`, {method: 'PUT', body: { id: tagToEditId.value, name: payload.name },})
        showEditTagForm.value = false
        tagToEditId.value = null
        await Promise.all([refreshTasks(), refreshTags()])
      }
      catch (err)
      {
        toastMessage.value = 'Error updating tag'
        showToast.value = true
      }
    }

    // Start tag delete dialog
    const confirmDeleteTag = (id: number) =>
    {
        tagToDeleteId.value = id
        showDeleteConfirmTag.value = true
    }

    // Delete tag
    const deleteTag = async () =>
        {
        if (!tagToDeleteId.value) return
        try
        {
            await $fetch(`/api/tags/${tagToDeleteId.value}`, { method: 'DELETE' })
            showDeleteConfirmTag.value = false
            const deleted = tagToDeleteId.value
            tagToDeleteId.value = null
            await Promise.all([refreshTasks(), refreshTags()])
            if (selectedTagIds.value.includes(deleted)) // clear filter if needed
            {
                selectedTagIds.value = selectedTagIds.value.filter(id => id !== deleted)
            }
        }
        catch (err)
        {
            toastMessage.value = 'Error deleting tag'
            showToast.value = true
        }
    }

    // Toggle task done
    const toggleDone = async (id: number) =>
    {
        try
        {
            await $fetch(`/api/tasks/${id}/done`, { method: 'PATCH' })
            await refreshTasks()
        }
        catch (err)
        {
            toastMessage.value = 'Error marking task as done'
            showToast.value = true
        }
    }

    // Initial fetch
    onMounted(() => {
        refreshTasks()
        refreshTags()
    })

    // -----------------
    // Outputs
    // -----------------

    return {
        tasks,
        tags,
        selectedTagIds,
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
        taskToEditId,
        taskToDeleteId,
        tagToDeleteId,
        filteredTasks,
        refreshTasks,
        refreshTags,
        handleCreateTask,
        startEditTask,
        handleEditTask,
        startCreateTag,
        handleCreateTag,
        startEditTag,
        handleEditTag,
        confirmDeleteTask,
        confirmDeleteTag,
        deleteTask,
        deleteTag,
        toggleDone,
        toggleTagFilter,
  }
}