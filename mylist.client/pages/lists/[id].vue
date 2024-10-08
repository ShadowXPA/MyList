<script lang="ts" setup>
import type { ListItem, UserList } from '~/types/myList'
const runtimeConfig = useRuntimeConfig()
const route = useRoute()
const id = route.params.id

const { data: list, refresh, status, error } = await useFetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists/${id}?include-items=true`)

const newItemModal = ref(false)
const newItem = ref<{ name: string, description?: string }>({ name: '' })
const editListModal = ref(false)
const editList = ref<{ name: string, description?: string }>({ name: list.value?.name ?? '', description: list.value?.description })
const deleteItemModal = ref(false)
const deleteItem = ref<{ id: number, name: string }>({ id: 0, name: '' })
const errorModal = ref(false)
const errorMsg = ref<string>()

const onCloseNewItemModal = () => {
    newItem.value.name = ''
    newItem.value.description = undefined
}

const closeNewItemModal = () => {
    newItemModal.value = false
    onCloseNewItemModal()
}

const addNewItem = async () => {
    if (!list.value) {
        closeNewItemModal()
        return
    }

    const data = await $fetch<ListItem>(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}/items`,
        {
            method: 'post',
            body: newItem.value
        }
    ).catch((error) => {
        let msg = `An error occured while adding new item:\n(${error.data.status}) ${error.data.title}\n`

        for (let i in error.data.errors) {
            for (let err of error.data.errors[i]) {
                msg += `\n${err}`
            }
        }

        errorMsg.value = msg
        errorModal.value = true
    })

    if (!data)
        return

    if (!list.value.items) {
        list.value.items = []
    }

    list.value.items.unshift(data)
    closeNewItemModal()
}

const onCloseEditListModal = () => {
    editList.value.name = list.value?.name ?? ''
    editList.value.description = list.value?.description
}

const closeEditListModal = () => {
    editListModal.value = false
    onCloseEditListModal()
}

const editCurrentList = async () => {
    if (!list.value) {
        closeEditListModal()
        return
    }

    const data = await $fetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value?.id}`,
        {
            method: 'patch',
            body: editList.value
        }
    )

    list.value = data

    closeEditListModal()
}

const onCloseDeleteItemModal = () => {
    deleteItem.value.id = 0
    deleteItem.value.name = ''
}

const closeDeleteItemModal = () => {
    deleteItemModal.value = false
    onCloseDeleteItemModal()
}

const deleteSelectedItem = async () => {
    if (!list.value || !list.value.items) {
        closeDeleteItemModal()
        return
    }

    await $fetch<ListItem>(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}/items/${deleteItem.value.id}`, { method: 'delete' })

    list.value.items = list.value.items.filter((item) => item.id !== deleteItem.value.id)
    closeDeleteItemModal()
}
</script>

<template>
    <div v-if="list">
        <Title>{{ list.name }}</Title>
        <div class="flex flex-col gap-4">
            <h1 class="py-10 mx-auto text-5xl font-bold">{{ list.name }}</h1>
            <p class="text-sm font-thin"><span class="font-bold">Created:</span> {{
                parseDate(list.createdAt).toLocaleString() }}</p>
            <p v-if="list.updatedAt" class="text-sm font-thin"><span class="font-bold">Updated:</span> {{
                parseDate(list.updatedAt).toLocaleString() }}</p>
            <p v-if="list.description" class="text-lg whitespace-pre-line">{{ list.description }}</p>
            <div class="flex gap-2 justify-end">
                <MyListButton title="Refresh" icon="bi:arrow-counterclockwise" @click="refresh"
                    class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                <MyListButton title="New item" icon="bi:plus-lg" @click="(e: any) => newItemModal = true"
                    class="bg-green-100 hover:bg-green-200 active:bg-green-300" />
                <MyListButton title="Edit list" icon="bi:pencil" @click="(e: any) => editListModal = true"
                    class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
            </div>
            <div v-if="status === 'success'" class="flex flex-col gap-2">
                <div v-for="item in list.items" :key="item.id"
                    class="p-4 flex gap-2 justify-between items-center border rounded cursor-pointer hover:bg-neutral-100">
                    <div class="flex flex-col gap-1">
                        <p class="text-xl font-bold">{{ item.name }}</p>
                        <p class="text-xs font-thin">{{ parseDate(item.createdAt).toLocaleString() }}</p>
                        <p v-if="item.description" class="whitespace-pre-line">{{ item.description }}</p>
                    </div>
                    <div>
                        <MyListButton icon="bi:trash" @click="(e: any) => {
                            deleteItem.id = item.id
                            deleteItem.name = item.name
                            deleteItemModal = true
                        }" class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
                    </div>
                </div>
            </div>
            <div v-else-if="status === 'idle'">Idle</div>
            <div v-else-if="status === 'pending'" class="mx-auto font-bold">Loading...</div>
            <div v-else-if="status === 'error' && error">
                <span class="text-red-500 font-bold">An error occured:</span>
                &nbsp;({{ error.statusCode }}) {{ error.message }}
            </div>
        </div>

        <MyListModal v-model="newItemModal" @closed="onCloseNewItemModal" static>
            <template #header>
                <p class="text-lg font-bold">Add a new item</p>
            </template>

            <div class="flex flex-col gap-4">
                <div>
                    <label for="item-name">Name:</label>
                    <input id="item-name" name="name" v-model="newItem.name" type="text"
                        class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50" />
                </div>
                <div>
                    <label for="item-description">Description:</label>
                    <textarea id="item-description" name="description" v-model="newItem.description" type="text"
                        class="transition-none mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50"></textarea>
                </div>
            </div>

            <template #footer>
                <MyListButton title="Add" @click="addNewItem"
                    class="ml-auto bg-green-100 hover:bg-green-200 active:bg-green-300" />
            </template>
        </MyListModal>

        <MyListModal v-model="editListModal" @closed="onCloseEditListModal" static>
            <template #header>
                <p class="text-lg font-bold">Edit "{{ list.name }}"</p>
            </template>

            <div class="flex flex-col gap-4">
                <div>
                    <label for="list-name">Name:</label>
                    <input id="list-name" name="name" v-model="editList.name" type="text"
                        class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50" />
                </div>
                <div>
                    <label for="list-description">Description:</label>
                    <textarea id="list-description" name="description" v-model="editList.description" type="text"
                        class="transition-none mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50"></textarea>
                </div>
            </div>

            <template #footer>
                <MyListButton title="Save" @click="editCurrentList"
                    class="ml-auto bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
            </template>
        </MyListModal>

        <MyListModal v-model="deleteItemModal" @closed="onCloseDeleteItemModal" static>
            <template #header>
                <p class="text-lg font-bold">Deleting "{{ deleteItem.name }}"</p>
            </template>

            <div>Are you sure you want to delete "{{ deleteItem.name }}"?</div>

            <template #footer>
                <MyListButton title="Cancel" @click="closeDeleteItemModal"
                    class="ml-auto bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                <MyListButton title="Delete" @click="deleteSelectedItem"
                    class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
            </template>
        </MyListModal>

        <MyListModal v-model="errorModal">
            <template #header>
                <p class="text-lg font-bold">Error!</p>
            </template>

            <div class="font-bold text-red-500 whitespace-pre-line">{{ errorMsg }}</div>
        </MyListModal>
    </div>
    <div v-else>
        <Title>List not found...</Title>
        <p class="text-lg text-red-500 font-bold text-center">The list you are trying to access does not exist...</p>
        <MyListButton title="Back" icon="bi:arrow-left"
            class="mt-4 mx-auto bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300"
            @click="(e: any) => navigateTo('/')" />
    </div>
</template>
