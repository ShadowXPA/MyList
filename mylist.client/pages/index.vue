<script lang="ts" setup>
import MyListSearch from '~/components/MyListSearch.vue';
import type { CurrentItem, UserList } from '~/types/myList'
import { parseDate } from '~/utils/dateUtils'

const searchQuery = ref<string>('')

const placeholders = [
    'plan to watch...',
    'groceries...',
    'watched...',
    'reading...',
    'playing...',
    'played...',
    'listened...',
    'to-do...'
]

const index = randomNumber(0, placeholders.length)

const runtimeConfig = useRuntimeConfig()
const { data: lists, refresh, status, error } = await useFetch<UserList[]>(`${runtimeConfig.public.apiBaseURL}/api/lists`, {
    query: { q: searchQuery }
})

const selectedList = ref<CurrentItem>({ id: 0, item: { name: '' } })

const newListModal = ref(false)
const deleteListModal = ref(false)
const errorModal = ref(false)
const errorMsg = ref<string>()

const resetSelectedList = () => {
    selectedList.value.id = 0
    selectedList.value.item.name = ''
    selectedList.value.item.description = undefined
}

const createNewList = async (newList: { name: string, description?: string }) => {
    if (!lists.value) {
        newListModal.value = false
        return
    }

    const data = await $fetch<UserList>(`${runtimeConfig.public.apiBaseURL}/api/lists`,
        {
            method: 'post',
            body: newList
        }
    ).catch((error) => {
        let msg = `An error occured while adding new list:\n(${error.data.status}) ${error.data.title}\n`

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

    lists.value.unshift(data)
    newListModal.value = false
}

const deleteSelectedList = async () => {
    if (!lists.value) {
        deleteListModal.value = false
        return
    }

    await $fetch(`${runtimeConfig.public.apiBaseURL}/api/lists/${selectedList.value.id}`, { method: 'delete' })

    lists.value = lists.value.filter((list) => list.id !== selectedList.value.id)
    deleteListModal.value = false
}
</script>

<template>
    <Title>My Lists</Title>
    <div class="flex flex-col gap-4">
        <MyListTitle title="My Lists"
            description="Create and edit your own lists (to-do lists, movie lists, book lists, music lists, etc.)" />
        <MyListSearchBar :placeholder="placeholders[index]" title="Lists" :num-items="lists?.length" @refresh="refresh"
            @search="(query: string) => searchQuery = query">
            <template #actions>
                <MyListButton title="New list" icon="bi:plus-lg" @click="(e: any) => newListModal = true"
                    class="bg-green-100 hover:bg-green-200 active:bg-green-300" />
            </template>
        </MyListSearchBar>
        <div v-if="status === 'success' || status === 'pending'" class="flex flex-col gap-2">
            <div v-if="status === 'pending'" class="mx-auto font-bold">Loading...</div>
            <div v-for="list in lists" :key="list.id" @click.stop="navigateTo(`/lists/${list.id}`)"
                class="p-4 flex gap-2 justify-between items-center border rounded cursor-pointer hover:bg-neutral-100 active:bg-neutral-200">
                <div class="flex flex-col gap-1 max-w-[calc(100%-4rem)] overflow-auto">
                    <p class="text-2xl font-bold">{{ list.name }}</p>
                    <p class="text-xs font-thin">
                        {{ parseDate(list.createdAt).toLocaleString() }}
                        <template v-if="list.updatedAt"> - {{ parseDate(list.updatedAt).toLocaleString() }}</template>
                    </p>
                    <p v-if="list.description" class="text-ellipsis overflow-hidden whitespace-nowrap">
                        {{ list.description }}
                    </p>
                </div>
                <div class="flex flex-col gap-2 justify-end items-center flex-wrap">
                    <MyListButton icon="bi:trash" @click="(e: any) => {
                        selectedList.id = list.id
                        selectedList.item.name = list.name
                        selectedList.item.description = list.description
                        deleteListModal = true
                    }" class="bg-red-100 hover:bg-red-200 active:bg-red-300" />
                </div>
            </div>
        </div>
        <div v-else-if="status === 'idle'">Idle</div>
        <div v-else-if="status === 'error' && error">
            <span class="text-red-500 font-bold">An error occured:</span> ({{ error.statusCode }}) {{ error.message }}
        </div>
    </div>

    <CreateItemModal v-model="newListModal" @create="createNewList" />

    <DeleteModal v-model="deleteListModal" :name="selectedList.item.name" @closed="resetSelectedList"
        @delete="deleteSelectedList" />

    <ErrorModal v-model="errorModal" :message="errorMsg" />
</template>
