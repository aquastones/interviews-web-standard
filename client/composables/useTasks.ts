import { ref, computed, onMounted } from 'vue'

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

export function useTasks()
{
    // Variables
    const tasks = ref<Task[]>([]) // Tasks array
    const tags = ref<Tag[]>([]) // Tags array
    const toastMessage = ref('') // Toast message

    const createInit = { name: '', description: '', tags: '' } // Init values for a create form
    const editInit = ref({ name: '', description: '', tags: '' }) // Init values for an edit form

    const showToast = ref(false) // Toast visibility
    const showCreateForm = ref(false) // Create form visibility
    const showEditForm = ref(false) // Edit form visibility
    const showDeleteConfirm = ref(false) // Delete confirmation visibility

    const taskToEditId = ref<number | null>(null) // Edit task id
    const taskToDeleteId = ref<number | null>(null) // Delete task id
    const selectedTagIds = ref<number[]>([]) // Filter by tags ids

    // Functions
    // Filter tags
    const filteredTasks = computed(() =>
    {
        if (!selectedTagIds.value.length) return tasks.value
        return tasks.value.filter(task =>
        task.tags.some(t => selectedTagIds.value.includes(t.id))
        )
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
            console.error(e)
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
            console.error(e)
            toastMessage.value = 'Error loading tags'
            showToast.value = true
        }
    }

    // Toggle which tags are active filters
    const toggleTagFilter = (tagId: number) =>
    {
        const idx = selectedTagIds.value.indexOf(tagId)
        if (idx === -1) selectedTagIds.value.push(tagId)
        else selectedTagIds.value.splice(idx, 1)
    }

    const handleCreate = async (payload: { name: string; description: string; tags: string }) => {
        try
        {
            const newTask = await $fetch<Task>('/api/tasks', {method: 'POST',body: { name: payload.name, description: payload.description },})
            await $fetch(`/api/tasks/${newTask.id}/tags-multiple`, {method: 'POST',body: { tagString: payload.tags },})
            showCreateForm.value = false
            await refreshTags()
            await refreshTasks()
        }
        catch (err)
        {
            console.error(err)
            toastMessage.value = 'Error creating task'
            showToast.value = true
        }
    }

    const startEdit = (task: Task) =>
    {
        taskToEditId.value = task.id
        editInit.value =
        {
            name: task.name,
            description: task.description || '',
            tags: task.tags.map(t => t.name).join(' '),
        }
        showEditForm.value = true
    }

    const handleEdit = async (payload: { name: string; description: string; tags: string }) =>
    {
        if (!taskToEditId.value) return
        try
        {
            await $fetch(`/api/tasks/${taskToEditId.value}`,{method: 'PUT', body: {id: taskToEditId.value, name: payload.name, description: payload.description},})
            await $fetch(`/api/tasks/${taskToEditId.value}/tags-multiple`, {method: 'POST', body: {tagString: payload.tags},})
            showEditForm.value = false
            taskToEditId.value = null
            await refreshTags()
            await refreshTasks()
        }
        catch (err)
        {
            console.error(err)
            toastMessage.value = 'Error updating task'
            showToast.value = true
        }
    }

    const confirmDelete = (id: number) =>
    {
        taskToDeleteId.value = id
        showDeleteConfirm.value = true
    }

    const deleteTask = async () =>
    {
        if (!taskToDeleteId.value) return
        try
        {
            await $fetch(`/api/tasks/${taskToDeleteId.value}`, { method: 'DELETE' })
            showDeleteConfirm.value = false
            taskToDeleteId.value = null
            await refreshTags()
            await refreshTasks()
        }
        catch (err)
        {
            console.error(err)
            toastMessage.value = 'Error deleting task'
            showToast.value = true
        }
    }

    const deleteTag = async (tagId: number) => {
        try
        {
            await $fetch(`/api/tags/${tagId}`, { method: 'DELETE' })
            await refreshTags()
            await refreshTasks()
            if (selectedTagIds.value.includes(tagId))
            {
                selectedTagIds.value = selectedTagIds.value.filter(id => id !== tagId)
            }
        }
        catch (err)
        {
            console.error(err)
            toastMessage.value = 'Error deleting tag'
            showToast.value = true
        }
    }

    const toggleDone = async (id: number) =>
    {
        try
        {
            await $fetch(`/api/tasks/${id}/done`, { method: 'PATCH' })
            await refreshTasks()
        }
        catch (err)
        {
            console.error(err)
            toastMessage.value = 'Error marking task as done'
            showToast.value = true
        }
    }

    // Initial fetch
    onMounted(() => {
        refreshTasks()
        refreshTags()
    })

    return {
        tasks,
        tags,
        selectedTagIds,
        toastMessage,
        showToast,
        showCreateForm,
        showEditForm,
        showDeleteConfirm,
        createInit,
        editInit,
        taskToEditId,
        taskToDeleteId,
        filteredTasks,
        refreshTasks,
        refreshTags,
        handleCreate,
        startEdit,
        handleEdit,
        confirmDelete,
        deleteTask,
        deleteTag,
        toggleDone,
        toggleTagFilter,
  }
}