<template>
    <div class="About">
        <h1>About PowerPlantMap</h1>
    </div>
</template>

<script>
export default {
    name: "About",

    head() {
        return {
            title: "About PowerPlantMap",
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
