import { ref } from 'vue'

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

    return {
        tasks,
        toastMessage,
        showToast,
        showCreateForm,
        showEditForm,
        showDeleteConfirm,
        createInit,
        editInit,
        taskToEditId,
        taskToDeleteId,
        refreshTasks,
        handleCreate,
        handleEdit,
        startEdit,
        confirmDelete,
        deleteTask,
        toggleDone,
    }
}