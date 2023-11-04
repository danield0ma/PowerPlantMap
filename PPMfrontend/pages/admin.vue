<template>
    <div class="Admin">
        <div v-for="currentPowerPlant in powerPlants" :key="currentPowerPlant.powerPlantId">
            <PowerPlantCard :powerPlant="currentPowerPlant"></PowerPlantCard>
        </div>
    </div>
</template>

<script>
import PowerPlantCard from "../components/PowerPlantCard.vue";
export default {
    name: "Admin",
    components: {PowerPlantCard},
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
        const powerPlants =  await res.json();
        return { powerPlants };
    },
};
</script>

<style>
body {
    margin: 0;
    padding: 0;
}

.Admin {
    background-color: #808080;
    overflow-y: auto;
    padding-top: 4rem;
    text-align: center;
    margin: auto;
}
</style>
