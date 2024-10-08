// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: false },
  modules: ['@nuxt/icon', '@nuxtjs/tailwindcss'],
  css: ['~/assets/css/main.css'],
  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1'
    }
  },
  runtimeConfig: {
    public: {
      apiBaseURL: '/mylist-api'
    }
  },
  nitro: {
    routeRules: {
      '/mylist-api/**': { proxy: import.meta.env.API_BASE_URL }
    }
  }
})
