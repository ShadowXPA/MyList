<script lang="ts" setup>
const props = defineProps<{ title: string, description?: string }>()
</script>

<template>
    <h1 class="py-10 text-center mx-auto text-5xl font-bold">{{ props.title }}</h1>
    <p v-if="props.description" class="text-xl text-center mx-auto whitespace-pre-line">
        {{ props.description }}
    </p>
    <div class="mb-16"></div>
</template>
