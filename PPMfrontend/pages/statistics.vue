<template></template>

<script>
export default {
    name: "Statistics",

    data() {
        return {
            accessToken: process.env.ACCESS_TOKEN,
        };
    },

    head() {
        return {
            title: "Statistics - PowerPlantMap",
        };
    },

    async asyncData() {
        const BASE_PATH = "https://powerplantmap.tech:5001/";
        const powers = await fetch(
            `${BASE_PATH}API/Power/getPowerOfPowerPlants`
        );
        const powerOfPowerPlants = await powers.json();

        return { gj, powerOfPowerPlants };
    },
};
</script>
