<script lang="ts" setup>
const props = defineProps<{ createdAt: string, updatedAt?: string }>()
</script>

<template>
    <p class="text-sm font-thin flex gap-2 flex-wrap justify-end items-center">
        <span class="whitespace-nowrap">
            <span class="font-bold">Created:</span> {{ parseDate(props.createdAt).toLocaleString() }}
        </span>
        <span v-if="props.updatedAt" class="whitespace-nowrap">
            <span class="font-bold">Updated:</span> {{ parseDate(props.updatedAt).toLocaleString() }}
        </span>
    </p>
</template>
