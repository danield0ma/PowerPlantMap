<template>
    <div class="Card">
        <h2>{{ this.powerPlant.name }}</h2>
        <p>{{ this.powerPlant.powerPlantId }}</p>
        <p>{{ this.powerPlant.description }}</p>
        <p>{{ this.powerPlant.latitude }}</p>
        <p>{{ this.powerPlant.longitude }}</p>
    </div>
</template>

<script>
export default {
    name: "PowerPlantCard",

    props: {
        powerPlant: Object
    },
};
</script>

<style>
.Card {
    background-color: white;
    border: 1px solid black;
    border-radius: 5px;
    padding: 1rem;
    margin: 1rem;
    width: 50vw;
}
</style>
