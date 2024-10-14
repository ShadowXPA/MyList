<script lang="ts" setup>
const props = defineProps<{ name: string }>()
const shown = defineModel<boolean>()
const emit = defineEmits(['closed', 'delete'])
</script>

<template>
    <MyListModal v-model="shown" @closed="emit('closed')" static>
        <template #header>
            <p class="text-lg font-bold">Deleting "{{ props.name }}"</p>
        </template>

        <div>Are you sure you want to delete "{{ props.name }}"?</div>

        <template #footer>
            <MyListButton title="Delete" @click="emit('delete')"
                class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
        </template>
    </MyListModal>
</template>
