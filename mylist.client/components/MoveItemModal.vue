<script lang="ts" setup>
import type { UserList } from '~/types/myList';

const props = defineProps<{ name: string }>()
const shown = defineModel<boolean>()
const emit = defineEmits<{
    closed: [],
    move: [listId: number]
}>()

const runtimeConfig = useRuntimeConfig()
const route = useRoute()
const id = parseInt(route.params.id as string)

const { data: lists, status} = await useFetch<UserList[]>(`${runtimeConfig.public.apiBaseURL}/api/lists`)

const listId = ref<number>(0)

const onClosed = () => {
    listId.value = 0
    emit('closed')
}
</script>

<template>
    <MyListModal v-model="shown" @closed="onClosed" static>
        <template #header>
            <p class="text-lg font-bold">Move "{{ props.name }}"</p>
        </template>

        <div class="flex justify-center items-center gap-4">
            <label for="new-list-name" class="flex-auto text-center">{{ props.name }}</label>
            <label for="new-list-name">
                <Icon name="bi:arrow-right" />
            </label>
            <select id="new-list-name" v-model="listId"
                class="flex-auto border rounded shadow-sm focus:border-gray-500 focus:ring focus:ring-sky-300 focus:ring-opacity-50">
                <template v-if="status === 'success'">
                    <option selected disabled value="0">Select new List</option>
                    <template v-for="list in lists">
                        <option v-if="list.id !== id" :key="list.id" :value="list.id">
                            {{ list.name }}
                        </option>
                    </template>
                </template>
                <template v-else>
                    <option selected disabled value="0">No Lists</option>
                </template>
            </select>
        </div>

        <template #footer>
            <MyListButton title="Move" @click="emit('move', listId)"
                class="bg-sky-100 hover:bg-sky-200 active:bg-sky-300" />
        </template>
    </MyListModal>
</template>
