<template>
    <div class="CardContainer">
        <div class="Card">
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
            <!-- <div v-if="showDetails">
                <label for="id">Erőmű azonosító:</label>
                <input
                    id="id"
                    type="text"
                    :placeholder="this.powerPlant.powerPlantId"
                />
                <p>{{ this.powerPlant.powerPlantId }}</p>
                <p>{{ this.powerPlant.name }}</p>
                <p>{{ this.powerPlant.description }}</p>
                <p>{{ this.powerPlant.operatorCompany }}</p>
                <p>{{ this.powerPlant.webpage }}</p>
                <p>{{ this.powerPlant.longitude }}</p>
                <p>{{ this.powerPlant.latitude }}</p>
                <p>{{ this.powerPlant.color }}</p>
                <p>{{ this.powerPlant.address }}</p>
                <p>{{ this.powerPlant.isCountry }}</p>
                <div
                    v-for="currentBloc in this.powerPlant.blocs"
                    :key="currentBloc.blocId"
                >
                    <p>{{ currentBloc.blocId }}</p>
                    <p>{{ currentBloc.blocType }}</p>
                    <p>{{ currentBloc.maxBlocCapacity }}</p>
                    <p>{{ currentBloc.commissionDate }}</p>
                    <div
                        v-for="currentGenerator in currentBloc.generators"
                        :key="currentGenerator.generatorId"
                    >
                        <p>{{ currentBloc.generatorId }}</p>
                        <p>{{ currentBloc.maxCapacity }}</p>
                    </div>
                </div>
            </div> -->
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

    methods: {
        deletePowerPlant() {
            console.log("Delete: " + this.powerPlant.powerPlantId);
        },
    },
};
</script>

<style>
.CardContainer {
    display: flex;
    justify-content: center;
}

.Card {
    background-color: white;
    border: 1px solid black;
    border-radius: 5px;
    padding: 1rem;
    margin: 0.25rem 0.5rem;
    min-height: 100px;
    display: flex;
    justify-content: space-around;
    align-items: center;
    box-shadow: 0 5px 5px rgba(0, 0, 0, 0.25);
}

.Card:hover {
    background-color: lightgray;
}

p {
    padding: 0 0.5rem;
    text-align: center;
    font-size: 18px;
}

.faicon {
    cursor: pointer;
    vertical-align: center;
    padding: 0.5rem;
}

.red {
    color: red;
}

.green {
    color: green !important;
}

.blue {
    color: blue !important;
}
</style>
