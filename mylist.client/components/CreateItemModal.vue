<script lang="ts" setup>
import type { Item } from '~/types/myList';

const shown = defineModel<boolean>()
const emit = defineEmits<{
    closed: [],
    create: [item: Item]
}>()

const newItem = ref<Item>({ name: '' })

const onClosed = () => {
    newItem.value.name = ''
    newItem.value.description = undefined
    emit('closed')
}
</script>

<template>
    <MyListModal v-model="shown" @closed="onClosed" static>
        <template #header>
            <p class="text-lg font-bold">Create a new item</p>
        </template>

        <div class="flex flex-col gap-4">
            <div>
                <label for="name">Name:</label>
                <input id="name" name="name" v-model="newItem.name" type="text"
                    class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50" />
            </div>
            <div>
                <label for="description">Description:</label>
                <textarea id="description" name="description" v-model="newItem.description"
                    class="transition-none mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50"></textarea>
            </div>
        </div>

        <template #footer>
            <MyListButton title="Create" @click="emit('create', newItem)"
                class="bg-green-100 hover:bg-green-200 active:bg-green-300" />
        </template>
    </MyListModal>
</template>
