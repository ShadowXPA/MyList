<script lang="ts" setup>
import type { ListItem, UserList } from '~/types/myList'
const runtimeConfig = useRuntimeConfig()
const route = useRoute()
const id = route.params.id

const searchQuery = ref<string>('')
const { data: list, refresh, status, error } = await useFetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists/${id}`, {
    query: { q: searchQuery, 'include-items': true }
})

const { data: lists, status: listsStatus } = await useFetch<UserList[]>(`${runtimeConfig.public.apiBaseURL}/api/lists`)

const newItemModal = ref(false)
const newItem = ref<{ name: string, description?: string }>({ name: '' })
const editListModal = ref(false)
const editList = ref<{ name: string, description?: string }>({ name: list.value?.name ?? '', description: list.value?.description })
const deleteItemModal = ref(false)
const currentItem = ref<{ id: number, name: string }>({ id: 0, name: '' })
const editItemModal = ref(false)
const editItem = ref<{ name: string, description?: string }>({ name: '', description: '' })
const moveItemModal = ref(false)
const newListId = ref<number>(0)
const errorModal = ref(false)
const errorMsg = ref<string>()

const resetCurrentItem = () => {
    currentItem.value.id = 0
    currentItem.value.name = ''
}

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

    const data = await $fetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}`,
        {
            method: 'patch',
            body: editList.value
        }
    )

    list.value = data

    closeEditListModal()
}

const onCloseEditItemModal = () => {
    editItem.value.name = ''
    editItem.value.description = ''
    resetCurrentItem()
}

const closeEditItemModal = () => {
    editItemModal.value = false
    onCloseEditItemModal()
}

const editCurrentItem = async () => {
    if (!list.value || !list.value.items) {
        closeEditItemModal()
        return
    }

    const data = await $fetch<ListItem>(`${runtimeConfig.public.apiBaseURL}/api/items/${currentItem.value.id}`,
        {
            method: 'patch',
            body: editItem.value
        }
    )

    list.value.items = list.value.items.filter((item) => item.id !== currentItem.value.id)
    list.value.items.unshift(data)

    closeEditItemModal()
}

const onCloseMoveItemModal = () => {
    newListId.value = 0
    resetCurrentItem()
}

const closeMoveItemModal = () => {
    moveItemModal.value = false
    onCloseMoveItemModal()
}

const moveCurrentItem = async () => {
    if (!list.value || !list.value.items) {
        closeEditItemModal()
        return
    }

    if (!newListId.value || newListId.value === 0) {
        return
    }

    await $fetch(`${runtimeConfig.public.apiBaseURL}/api/items/${currentItem.value.id}/move/${newListId.value}`, { method: 'post' })

    list.value.items = list.value.items.filter((item) => item.id !== currentItem.value.id)

    closeMoveItemModal()
}

const onCloseDeleteItemModal = () => {
    resetCurrentItem()
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

    await $fetch(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}/items/${currentItem.value.id}`, { method: 'delete' })

    list.value.items = list.value.items.filter((item) => item.id !== currentItem.value.id)
    closeDeleteItemModal()
}
</script>

<template>
    <div v-if="list">
        <Title>{{ list.name }}</Title>
        <div class="flex flex-col gap-4">
            <h1 class="py-10 text-center mx-auto text-5xl font-bold">{{ list.name }}</h1>
            <p v-if="list.description" class="text-xl whitespace-pre-line">{{ list.description }}</p>
            <p class="text-sm mt-16 font-thin flex gap-2 flex-wrap justify-end items-center">
                <span class="whitespace-nowrap">
                    <span class="font-bold">Created:</span> {{ parseDate(list.createdAt).toLocaleString() }}
                </span>
                <span v-if="list.updatedAt" class="whitespace-nowrap">
                    <span class="font-bold">Updated:</span> {{ parseDate(list.updatedAt).toLocaleString() }}
                </span>
            </p>
            <div class="sticky bg-white top-0 py-2 z-10 flex flex-col gap-2">
                <div class="flex gap-x-40 gap-y-4 justify-between items-center flex-wrap">
                    <div class="flex-auto">
                        <MyListSearch @search="(query: string) => searchQuery = query" />
                    </div>
                    <div class="flex-auto flex gap-2 justify-end items-center flex-wrap">
                        <MyListButton title="Refresh" icon="bi:arrow-counterclockwise" @click="refresh"
                            class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                        <MyListButton title="New item" icon="bi:plus-lg" @click="(e: any) => newItemModal = true"
                            class="bg-green-100 hover:bg-green-200 active:bg-green-300" />
                        <MyListButton title="Edit list" icon="bi:pencil" @click="(e: any) => editListModal = true"
                            class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
                    </div>
                </div>
                <div class="ml-auto text-sm text-neutral-500 font-bold">{{ list.items?.length ?? 0 }} Items</div>
            </div>
            <div v-if="status === 'success'" class="flex flex-col gap-2">
                <div v-for="item in list.items" :key="item.id"
                    class="p-4 flex gap-2 justify-between items-center border rounded hover:bg-neutral-100">
                    <div class="flex flex-col gap-1">
                        <p class="text-2xl font-bold">{{ item.name }}</p>
                        <p class="text-xs font-thin">
                            {{ parseDate(item.createdAt).toLocaleString() }}
                            <span v-if="item.updatedAt"> - {{ parseDate(item.updatedAt).toLocaleString() }}</span>
                        </p>
                        <p v-if="item.description" class="whitespace-pre-line">{{ item.description }}</p>
                    </div>
                    <div class="flex gap-2">
                        <MyListButton icon="bi:arrow-left-right" @click="(e: any) => {
                            currentItem.id = item.id
                            currentItem.name = item.name
                            moveItemModal = true
                        }" class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                        <MyListButton icon="bi:pencil" @click="(e: any) => {
                            currentItem.id = item.id
                            currentItem.name = item.name
                            editItem.name = item.name
                            editItem.description = item.description
                            editItemModal = true
                        }" class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
                        <MyListButton icon="bi:trash" @click="(e: any) => {
                            currentItem.id = item.id
                            currentItem.name = item.name
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

        <MyListModal v-model="editItemModal" @closed="onCloseEditItemModal" static>
            <template #header>
                <p class="text-lg font-bold">Edit "{{ currentItem.name }}"</p>
            </template>

            <div class="flex flex-col gap-4">
                <div>
                    <label for="list-name">Name:</label>
                    <input id="list-name" name="name" v-model="editItem.name" type="text"
                        class="mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50" />
                </div>
                <div>
                    <label for="list-description">Description:</label>
                    <textarea id="list-description" name="description" v-model="editItem.description" type="text"
                        class="transition-none mt-1 block w-full border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50"></textarea>
                </div>
            </div>

            <template #footer>
                <MyListButton title="Save" @click="editCurrentItem"
                    class="ml-auto bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
            </template>
        </MyListModal>

        <MyListModal v-model="moveItemModal" @closed="onCloseMoveItemModal" static>
            <template #header>
                <p class="text-lg font-bold">Move "{{ currentItem.name }}"</p>
            </template>

            <div class="flex justify-center items-center gap-4">
                <label for="new-list-name" class="flex-auto text-center">{{ currentItem.name }}</label>
                <label for="new-list-name">
                    <Icon name="bi:arrow-right" />
                </label>
                <select id="new-list-name" v-model="newListId"
                    class="flex-auto border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50">
                    <template v-if="listsStatus === 'success'">
                        <option selected disabled value="0">Select new List</option>
                        <template v-for="newList in lists">
                            <option v-if="newList.id !== list.id" :key="newList.id" :value="newList.id">{{ newList.name }}</option>
                        </template>
                    </template>
                    <template v-else>
                        <option selected disabled value="0">No Lists</option>
                    </template>
                </select>
            </div>

            <template #footer>
                <MyListButton title="Move" @click="moveCurrentItem"
                    class="ml-auto bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
            </template>
        </MyListModal>

        <MyListModal v-model="deleteItemModal" @closed="onCloseDeleteItemModal" static>
            <template #header>
                <p class="text-lg font-bold">Deleting "{{ currentItem.name }}"</p>
            </template>

            <div>Are you sure you want to delete "{{ currentItem.name }}"?</div>

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
