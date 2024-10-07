<script lang="ts" setup>
import type { UserList } from '~/types/myList'
import { parseDate } from '~/utils/dateUtils'

const runtimeConfig = useRuntimeConfig()
const { data: lists, refresh, status, error } = await useFetch<UserList[]>(`${runtimeConfig.public.apiBaseUrl}/api/lists`)

const newListModal = ref(false)
const newList = ref<{ name: string, description?: string }>({ name: '' })
const deleteListModal = ref(false)
const deleteList = ref<{ id: number, name: string }>({ id: 0, name: '' })

const onCloseNewListModal = () => {
    newList.value.name = ''
    newList.value.description = undefined
}

const closeNewListModal = () => {
    newListModal.value = false
    onCloseNewListModal()
}

const addNewList = () => {
    // TODO: add new list, if successful refresh data, close modal, etc.

    closeNewListModal()
}

const onCloseDeleteListModal = () => {
    deleteList.value.id = 0
    deleteList.value.name = ''
}

const closeDeleteListModal = () => {
    deleteListModal.value = false
    onCloseDeleteListModal()
}

const deleteSelectedList = () => {
    // TODO: delete the list, if successful refresh data, close modal, etc.

    closeDeleteListModal()
}
</script>

<template>
    <Title>Your lists</Title>
    <div class="flex flex-col gap-4">
        <h1 class="py-10 mx-auto text-5xl font-bold">Your lists</h1>
        <div class="flex gap-2 justify-end">
            <MyListButton title="Refresh" icon="bi:arrow-counterclockwise" @click="refresh"
                class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
            <MyListButton title="New list" icon="bi:plus-lg" @click="(e: any) => newListModal = true"
                class="bg-green-100 hover:bg-green-200 active:bg-green-300" />
        </div>
        <div v-if="status === 'success'" class="flex flex-col gap-2">
            <div v-for="list in lists" :key="list.id" @click.stop="navigateTo(`/lists/${list.id}`)"
                class="p-4 flex gap-2 justify-between items-center border rounded cursor-pointer hover:bg-neutral-100 active:bg-neutral-200">
                <div class="flex flex-col gap-1 max-w-[calc(100%-4rem)]">
                    <p class="text-xl font-bold">{{ list.name }}</p>
                    <p class="text-xs font-thin">
                        {{ parseDate(list.createdAt).toLocaleString() }}
                        <span v-if="list.updatedAt"> - {{ parseDate(list.updatedAt).toLocaleString() }}</span>
                    </p>
                    <p v-if="list.description" class="text-ellipsis overflow-hidden whitespace-nowrap">{{
                        list.description }}</p>
                </div>
                <div>
                    <MyListButton icon="bi:trash" @click="(e: any) => {
                        deleteList.id = list.id
                        deleteList.name = list.name
                        deleteListModal = true
                    }" class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
                </div>
            </div>
        </div>
        <div v-else-if="status === 'idle'">Idle</div>
        <div v-else-if="status === 'pending'" class="mx-auto font-bold">Loading...</div>
        <div v-else-if="status === 'error' && error">
            <span class="text-red-500 font-bold">An error occured:</span> ({{ error.statusCode }}) {{ error.message }}
        </div>
    </div>

    <MyListModal v-model="newListModal" @closed="onCloseNewListModal" static>
        <template #header>
            <p class="text-lg font-bold">Add a new list</p>
        </template>

        <div class="flex flex-col gap-4">
            <div>
                <label for="list-name">Name:</label>
                <input id="list-name" name="name" v-model="newList.name" type="text"
                    class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50" />
            </div>
            <div>
                <label for="list-description">Description:</label>
                <textarea id="list-description" name="description" v-model="newList.description" type="text"
                    class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50"></textarea>
            </div>
        </div>

        <template #footer>
            <MyListButton title="Add" @click="addNewList"
                class="ml-auto bg-green-100 hover:bg-green-200 active:bg-green-300" />
        </template>
    </MyListModal>

    <MyListModal v-model="deleteListModal" @closed="onCloseDeleteListModal" static>
        <template #header>
            <p class="text-lg font-bold">Deleting "{{ deleteList.name }}"</p>
        </template>

        <div>Are you sure you want to delete "{{ deleteList.name }}"?</div>

        <template #footer>
            <MyListButton title="Cancel" @click="closeDeleteListModal"
                class="ml-auto bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
            <MyListButton title="Delete" @click="deleteSelectedList"
                class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
        </template>
    </MyListModal>
</template>
