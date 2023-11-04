<template>
    <div class="Admin">
        <div class="container">
            <div class="Card">
                <h3>Új létrehozása</h3>
                <font-awesome-icon
                    :icon="['fas', 'add']"
                    :size="iconSize"
                    class="faicon green"
                    v-on:click="toggleShowDetails"
                />
            </div>
        </div>
        <div
            v-for="currentPowerPlant in powerPlants"
            :key="currentPowerPlant.powerPlantId"
        >
            <PowerPlantCard :powerPlant="currentPowerPlant"></PowerPlantCard>
        </div>
    </div>
</template>

<script>
import PowerPlantCard from "../../components/PowerPlantCard.vue";
export default {
    name: "PowerPlantEditor",
    layout: "adminLayout",
    components: { PowerPlantCard },
    head() {
        return {
            title: "PowerPlant Editor - PowerPlantMap",
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
        return { powerPlants };
    },
};
</script>

<style scoped>
body {
    margin: 0;
    padding: 0;
}

.Admin {
    background-color: #808080;
    overflow-y: auto;
    margin-top: 3.5rem;
    height: calc(100vh - 3.5rem);
    justify-content: center;
}

.container {
    display: flex;
    justify-content: center;
}
</style>
