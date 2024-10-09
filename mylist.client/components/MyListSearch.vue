<script lang="ts" setup>
const props = defineProps<{ placeholder?: string }>()
const query = ref<string>('')
const emit = defineEmits(['search'])

const emitSearch = () => {
    emit('search', query.value)
}
</script>

<template>
    <div class="border rounded bg-neutral-100 py-1 px-2 flex gap-2 justify-center items-center">
        <label for="search"><Icon name="bi:search" /></label>
        <input id="search" v-model="query" :placeholder="props.placeholder ?? 'Search...'" @keypress.stop="(e: KeyboardEvent) => { if (e.key === 'Enter') { emitSearch() } }" class="bg-neutral-100 border-0 rounded w-full" type="text" />
        <MyListButton title="Search" class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" @click.stop="emitSearch" />
    </div>
</template>
