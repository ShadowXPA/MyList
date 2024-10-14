<script lang="ts" setup>
const props = defineProps<{ message?: string }>()
const shown = defineModel<boolean>()
</script>

<template>
    <MyListModal v-model="shown">
        <template #header>
            <p class="text-lg font-bold">Error!</p>
        </template>

        <div class="font-bold text-red-500 whitespace-pre-line">{{ props.message }}</div>
    </MyListModal>
</template>
