<template>
    <div class="min-h-screen flex flex-col items-center justify-center bg-gray-900 text-white font-mono text-center">

        <h1 class="text-6xl font-bold mb-20">
            Welcome to Task Manager
        </h1>

        <NuxtLink to="/tasks"
            class="text-3xl px-12 py-6 bg-sky-600 hover:bg-sky-300 text-white rounded-2xl transition-colors duration-500">
            Go to tasks
        </NuxtLink>

    </div>
</template>
