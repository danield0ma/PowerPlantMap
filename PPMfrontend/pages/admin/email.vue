<template>
    <div class="About">
        <h1>PowerPlantMap Email lista</h1>
    </div>
</template>

<script>
export default {
    name: "Email",
    layout: "adminLayout",
    middleware: "authenticated",
    head() {
        return {
            title: "Email szerkesztő - PowerPlantMap",
        };
    },
};
</script>

<style>
body {
    margin: 0;
    padding: 0;
}

.About {
    background-color: #808080;
    overflow-y: auto;
    padding-top: 4rem;
    text-align: center;
    margin: auto;
}
</style>
