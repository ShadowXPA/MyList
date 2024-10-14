<script lang="ts" setup>
import type { Item } from '~/types/myList';

const props = defineProps<{ item: Item }>()
const shown = defineModel<boolean>()
const emit = defineEmits<{
    closed: [],
    edit: [item: Item]
}>()

const editItem = ref<Item>({ name: props.item.name, description: props.item.description })

onBeforeUpdate(() => {
    editItem.value.name = props.item.name
    editItem.value.description = props.item.description
})

const onClosed = () => {
    emit('closed')
}
</script>

<template>
    <MyListModal v-model="shown" @closed="onClosed" static>
        <template #header>
            <p class="text-lg font-bold">Edit "{{ props.item.name }}"</p>
        </template>

        <div class="flex flex-col gap-4">
            <div>
                <label for="name">Name:</label>
                <input id="name" name="name" v-model="editItem.name" type="text"
                    class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50" />
            </div>
            <div>
                <label for="description">Description:</label>
                <textarea id="description" name="description" v-model="editItem.description"
                    class="transition-none mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50"></textarea>
            </div>
        </div>

        <template #footer>
            <MyListButton title="Save" @click="emit('edit', editItem)"
                class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
        </template>
    </MyListModal>
</template>
