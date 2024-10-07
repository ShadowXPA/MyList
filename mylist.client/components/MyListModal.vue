<script lang="ts" setup>
const props = defineProps<{ static?: boolean }>()
const shown = defineModel<boolean>()
const emit = defineEmits(['closed'])
</script>

<template>
    <Body v-if="shown" style="overflow: hidden;" />
    <Teleport to="#modals">
        <Transition name="slide">
            <div v-if="shown" class="modal fixed w-full h-full overflow-auto backdrop-brightness-50 z-[1055]"
                @click.self="() => { if (!props.static) { shown = false; emit('closed') } }">
                <div class="modal-dialog relative max-w-xl my-10 mx-4 sm:mx-auto">
                    <div class="modal-content bg-white rounded shadow-lg">
                        <div class="modal-header p-2 border-b flex gap-2 items-center">
                            <slot name="header" />
                            <MyListButton icon="bi:x" @click="shown = false; emit('closed')"
                                class="ml-auto bg-neutral-100 hover:bg-neutral-200 active:bg-neutral-300" />
                        </div>
                        <div class="modal-body py-4 px-2">
                            <slot />
                        </div>
                        <div v-if="$slots.footer" class="modal-footer p-2 border-t flex gap-2 items-center">
                            <slot name="footer" />
                        </div>
                    </div>
                </div>
            </div>
        </Transition>
    </Teleport>
</template>

<style>
.slide-enter-from .modal-content,
.slide-leave-to .modal-content {
    transform: translateY(-20rem);
}
</style>
