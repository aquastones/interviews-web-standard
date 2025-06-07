// https://nuxt.com/docs/api/configuration/nuxt-config

import tailwindcss from "@tailwindcss/vite";

export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },
  css: ["~/assets/main.css"],
  vite: {
    server: {
      proxy: {
        '/api': {
          target: 'http://localhost:5283',
          changeOrigin: true,
          rewrite: path => path.replace(/^\/api/, '/api'),
        },
      },
    },
    plugins: [
      tailwindcss(),
    ],
  },
});