<script lang="ts" setup>
const props = defineProps<{ title?: string, numItems?: number, placeholder?: string }>()
const emit = defineEmits<{
    search: [query: string],
    refresh: []
}>()
</script>

<template>
    <div class="sticky bg-white top-0 py-2 z-10 flex flex-col gap-2">
        <div class="flex gap-x-40 gap-y-4 justify-between items-center flex-wrap">
            <div class="flex-auto">
                <MyListSearch :placeholder="placeholder" @search="(q: string) => emit('search', q)" />
            </div>
            <div class="flex-auto flex gap-2 justify-end items-center flex-wrap">
                <MyListButton title="Refresh" icon="bi:arrow-counterclockwise" @click="emit('refresh')"
                    class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                <slot name="actions" />
            </div>
        </div>
        <div class="ml-auto text-sm text-neutral-500 font-bold">{{ props.numItems ?? 0 }} {{ props.title ?? 'Items' }}</div>
    </div>
</template>
