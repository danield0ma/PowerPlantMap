<template>
    <div class="cardContainer">
        <div class="card extendedCard cardHover">
            <img
                :src="'/' + this.powerPlant.image"
                width="45rem"
                height="45rem"
            />
            <p>{{ this.powerPlant.description }}</p>
            <div style="display: flex; flex-direction: column">
                <font-awesome-icon
                    :icon="['fas', 'trash']"
                    :size="'lg'"
                    class="faicon red"
                    v-on:click="deletePowerPlant"
                />
                <font-awesome-icon
                    :icon="['fas', 'edit']"
                    :size="'lg'"
                    class="faicon green"
                    v-on:click="showModal = true"
                />
            </div>
        </div>
        <PowerPlantModal
            :powerPlant="this.powerPlant"
            v-if="showModal"
            @close="showModal = false"
        />
    </div>
</template>

<script>
import PowerPlantModal from "./PowerPlantModal";
export default {
    name: "PowerPlantCard",

    props: {
        powerPlant: Object,
    },

    components: { PowerPlantModal },

    data() {
        return {
            iconSize: "16px",
            showModal: false,
        };
    },

    mounted() {
        this.$parent.$on("openModal", this.handleModalOpeningFromParent);
    },

    methods: {
        handleModalOpeningFromParent() {
            this.showModal = true;
        },

        async deletePowerPlant() {
            if (
                window.confirm(
                    "Are you sure you want to delete this power plant?"
                )
            ) {
                console.log("Delete: " + this.powerPlant.powerPlantId);
                await this.$axios.$delete(
                    `api/PowerPlant/Delete?id=${this.powerPlant.powerPlantId}`
                );
            }
        },
    },
};
</script>

<style scoped>
p {
    padding: 0 0.5rem;
    text-align: center;
    font-size: 18px;
    margin: 0;
}

.extendedCard {
    display: flex;
    justify-content: space-around;
    align-items: center;
    flex-direction: row;
}
</style>
