<template>
    <div class="card cardHover">
        <StatModal
            v-if="showModal"
            @close="showModal = false"
            :powerPlant="powerPlant"
        ></StatModal>

        <h4>{{ compactPowerPlant.powerPlantDescription }}</h4>
        <StatItem :item="compactPowerPlant"></StatItem>
        <!-- <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'bolt']"
                :size="'lg'"
                class="faicon yellow"
            />
            <p>{{ compactPowerPlant.averagePower }} MW</p>
        </div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'lightbulb']"
                :size="'lg'"
                class="faicon blue"
            />
            <p>{{ compactPowerPlant.generatedEnergy }} MWh</p>
        </div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'percent']"
                :size="'lg'"
                class="faicon green"
            />
            <p>{{ compactPowerPlant.averageUsage }}%</p>
        </div> -->
        <font-awesome-icon
            :icon="['fas', 'circle-info']"
            :size="'lg'"
            class="faicon green"
            v-on:click="showModal = true"
        />
    </div>
</template>

<script>
import StatModal from "./StatModal";
import StatItem from "./StatItem";

export default {
    name: "StatsCard",

    props: {
        compactPowerPlant: Object,
        powerPlant: [],
    },

    components: { StatModal, StatItem },

    data() {
        return {
            showModal: false,
        };
    },
};
</script>
