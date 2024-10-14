<script lang="ts" setup>
import type { ListItem, Item, UserList, CurrentItem } from '~/types/myList'
const runtimeConfig = useRuntimeConfig()
const route = useRoute()
const id = route.params.id

const searchQuery = ref<string>('')
const { data: list, refresh, status, error } = await useFetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists/${id}`, {
    query: { q: searchQuery, 'include-items': true }
})

const selectedItem = ref<CurrentItem>({ id: 0, item: { name: '' } })

const newItemModal = ref(false)
const editListModal = ref(false)
const editItemModal = ref(false)
const moveItemModal = ref(false)
const deleteItemModal = ref(false)
const errorModal = ref(false)

const errorMsg = ref<string>()

const resetSelectedItem = () => {
    selectedItem.value.id = 0
    selectedItem.value.item.name = ''
    selectedItem.value.item.description = undefined
}

const createNewItem = async (newItem: Item) => {
    if (!list.value) {
        newItemModal.value = false
        return
    }

    const data = await $fetch<ListItem>(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}/items`,
        {
            method: 'post',
            body: newItem
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
    newItemModal.value = false
}

const editSelectedList = async (editList: Item) => {
    if (!list.value) {
        editListModal.value = false
        return
    }

    const data = await $fetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}`,
        {
            method: 'patch',
            body: editList
        }
    )

    list.value = data
    editListModal.value = false
}

const editSelectedItem = async (editItem: Item) => {
    if (!list.value || !list.value.items) {
        editItemModal.value = false
        return
    }

    const data = await $fetch<ListItem>(`${runtimeConfig.public.apiBaseURL}/api/items/${selectedItem.value.id}`,
        {
            method: 'patch',
            body: editItem
        }
    )

    list.value.items = list.value.items.filter((item) => item.id !== selectedItem.value.id)
    list.value.items.unshift(data)
    editItemModal.value = false
}

const moveSelectedItem = async (listId: number) => {
    if (!list.value || !list.value.items) {
        moveItemModal.value = false
        return
    }

    if (!listId || listId === 0) {
        return
    }

    await $fetch(`${runtimeConfig.public.apiBaseURL}/api/items/${selectedItem.value.id}/move/${listId}`, { method: 'post' })

    list.value.items = list.value.items.filter((item) => item.id !== selectedItem.value.id)
    moveItemModal.value = false
}

const deleteSelectedItem = async () => {
    if (!list.value || !list.value.items) {
        deleteItemModal.value = false
        return
    }

    await $fetch(`${runtimeConfig.public.apiBaseURL}/api/lists/${list.value.id}/items/${selectedItem.value.id}`, { method: 'delete' })

    list.value.items = list.value.items.filter((item) => item.id !== selectedItem.value.id)
    deleteItemModal.value = false
}
</script>

<template>
    <div v-if="list">
        <Title>{{ list.name }}</Title>
        <div class="flex flex-col gap-4">
            <MyListTitle :title="list.name" :description="list.description" />
            <MyListDates :created-at="list.createdAt" :updated-at="list.updatedAt" />
            <MyListSearchBar :num-items="list.items?.length" @refresh="refresh" @search="(query: string) => searchQuery = query">
                <template #actions>
                    <MyListButton title="New item" icon="bi:plus-lg" @click="(e: any) => newItemModal = true"
                        class="bg-green-100 hover:bg-green-200 active:bg-green-300" />
                    <MyListButton title="Edit list" icon="bi:pencil" @click="(e: any) => editListModal = true"
                        class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
                </template>
            </MyListSearchBar>
            <div v-if="status === 'success' || status === 'pending'" class="flex flex-col gap-2">
                <div v-if="status === 'pending'" class="mx-auto font-bold">Loading...</div>
                <div v-for="item in list.items" :key="item.id"
                    class="p-4 flex gap-2 justify-between items-center border rounded hover:bg-neutral-100">
                    <div class="flex flex-col gap-1 max-w-[calc(100%-4rem)] overflow-auto">
                        <p class="text-2xl font-bold">{{ item.name }}</p>
                        <p class="text-xs font-thin">
                            {{ parseDate(item.createdAt).toLocaleString() }}
                            <template v-if="item.updatedAt"> - {{ parseDate(item.updatedAt).toLocaleString() }}</template>
                        </p>
                        <p v-if="item.description" class="whitespace-pre-line">{{ item.description }}</p>
                    </div>
                    <div class="flex flex-col gap-2 justify-end items-center flex-wrap">
                        <MyListButton icon="bi:arrow-left-right" @click="(e: any) => {
                            selectedItem.id = item.id
                            selectedItem.item.name = item.name
                            selectedItem.item.description = item.description
                            moveItemModal = true
                        }" class="bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                        <MyListButton icon="bi:pencil" @click="(e: any) => {
                            selectedItem.id = item.id
                            selectedItem.item.name = item.name
                            selectedItem.item.description = item.description
                            editItemModal = true
                        }" class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
                        <MyListButton icon="bi:trash" @click="(e: any) => {
                            selectedItem.id = item.id
                            selectedItem.item.name = item.name
                            selectedItem.item.description = item.description
                            deleteItemModal = true
                        }" class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
                    </div>
                </div>
            </div>
            <div v-else-if="status === 'idle'">Idle</div>
            <div v-else-if="status === 'error' && error">
                <span class="text-red-500 font-bold">An error occured:</span>
                &nbsp;({{ error.statusCode }}) {{ error.message }}
            </div>
        </div>

        <CreateItemModal v-model="newItemModal" @create="createNewItem" />

        <EditItemModal v-model="editListModal" :item="list" @edit="editSelectedList" />

        <EditItemModal v-model="editItemModal" :item="selectedItem.item" @closed="resetSelectedItem"
            @edit="editSelectedItem" />

        <MoveItemModal v-model="moveItemModal" :name="selectedItem.item.name" @closed="resetSelectedItem"
            @move="moveSelectedItem" />

        <DeleteModal v-model="deleteItemModal" :name="selectedItem.item.name" @closed="resetSelectedItem"
            @delete="deleteSelectedItem" />

        <ErrorModal v-model="errorModal" :message="errorMsg" />
    </div>
    <div v-else>
        <Title>List not found...</Title>
        <p class="text-lg text-red-500 font-bold text-center">The list you are trying to access does not exist...</p>
        <MyListButton title="Back" icon="bi:arrow-left"
            class="mt-4 mx-auto bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300"
            @click="(e: any) => navigateTo('/')" />
    </div>
</template>
