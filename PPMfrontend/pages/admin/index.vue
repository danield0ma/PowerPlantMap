<template>
    <div class="About">
        <h1>Admin Index</h1>
    </div>
</template>

<script>
export default {
    name: "AdminIndex",
    layout: "adminLayout",
    head() {
        return {
            title: "Admin - PowerPlantMap",
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
