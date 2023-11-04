<template>
    <div>
        <div style="height: 3.5rem; position: absolute"></div>
        <h1>Admin</h1>
        <div v-for="powerPlant in powerPlants" :key="powerPlant.powerPlantId">
            <h2>{{ powerPlant.name }}</h2>
            <p>{{ powerPlant.powerPlantId }}</p>
            <p>{{ powerPlant.description }}</p>
            <p>{{ powerPlant.latitude }}</p>
            <p>{{ powerPlant.longitude }}</p>
        </div>
    </div>
</template>

<script>
export default {
    name: "Admin",

    head() {
        return {
            title: "Admin View - PowerPlantMap",
        };
    },

    data() {
        return {
            powerPlants: [],
            BASE_PATH: "https://powerplantmap.tech:5001/",
        };
    },

    async asyncData() {
        const BASE_PATH = "https://powerplantmap.tech:5001/";
        const res = await fetch(`${BASE_PATH}API/PowerPlant/Get`);
        const powerPlants = await res.json();
        return powerPlants;
    },
};
</script>

<style>
body {
    margin: 0;
    padding: 0;
}
</style>
