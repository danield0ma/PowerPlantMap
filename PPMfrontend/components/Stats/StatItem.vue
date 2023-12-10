<template>
    <div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'bolt']"
                :size="'lg'"
                class="faicon yellow"
            />
            <p>{{ item.averagePower }} MW</p>
        </div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'lightbulb']"
                :size="'lg'"
                class="faicon blue"
            />
            <p>{{ item.generatedEnergy }} MWh</p>
        </div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'percent']"
                :size="'lg'"
                class="faicon green"
            />
            <p>{{ item.averageUsage }}%</p>
        </div>
    </div>
</template>

<script>
export default {
    name: "StatItem",
    props: {
        item: {},
    },
};
</script>

<style scoped>
p {
    padding: 0 0.5rem;
    text-align: center;
    font-size: 16px;
    margin: 0;
}
</style>
