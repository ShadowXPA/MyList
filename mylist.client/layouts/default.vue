<template>
    <div class="container mx-auto font-sans">
        <MyListHeader />
        <main class="py-8 px-2">
            <slot />
        </main>
        <MyListFooter />
    </div>
</template>
