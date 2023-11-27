<template>
    <div class="card cardHover">
        <p>{{ powerPlant.powerPlantId }}</p>
        <p>{{ powerPlant.powerPlantDescription }}</p>
        <p>{{ powerPlant.averagePower }}</p>
        <p>{{ powerPlant.generatedEnergy }}</p>
    </div>
</template>

<script>
export default {
    name: "StatsCard",
    props: {
        powerPlant: Object,
    },
};
</script>

<style></style>
