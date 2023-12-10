<template>
    <div>
        <div class="modal-backdrop" v-on:click="$emit('close')"></div>
        <div class="shade-content"></div>
        <div class="modal-content" v-on:click.stop>
            <div class="modal-body">
                <h1>
                    {{ powerPlant[0].powerPlantDescription }} részletes
                    statisztikái
                </h1>
                <div class="grid align-items-center">
                    <div
                        v-for="generator in powerPlant"
                        :key="generator.generatorId"
                    >
                        <div class="card cardHover">
                            <h5 class="mb-3">{{ generator.generatorId }}</h5>
                            <StatItem :item="generator"></StatItem>
                        </div>
                    </div>
                </div>
                <p>{{ powerPlant.powerPlantId }}</p>
                <button
                    v-on:click="$emit('close')"
                    class="btn btn-outline-secondary mr-3"
                >
                    Bezárás
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import StatItem from "./StatItem";

export default {
    name: "StatModal",

    props: {
        powerPlant: [],
    },

    components: { StatItem },

    mounted() {
        console.log(this.powerPlant);
    },

    methods: {},
};
</script>

<style scoped>
h5 {
    margin: 0;
}
</style>
