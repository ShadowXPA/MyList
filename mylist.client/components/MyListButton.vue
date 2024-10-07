<script lang="ts" setup>
const props = defineProps<{ title?: string, icon?: string }>()
const emit = defineEmits(['click'])
</script>

<template>
    <button class="p-2 flex gap-1 items-center border rounded" type="button" @click.stop="(e: any) => emit('click', e)">
        <Icon v-if="icon" :name="icon" />
        <span v-if="title">{{ props.title }}</span>
    </button>
</template>
