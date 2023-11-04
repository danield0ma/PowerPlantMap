<template>
    <div class="CardContainer">
        <div class="Card">
            <div style="display: flex; justify-content: space-between">
                <div style="display: flex; align-items: center">
                    <img
                        :src="'/' + this.powerPlant.image"
                        width="35rem"
                        height="35rem"
                    />
                    <p>{{ this.powerPlant.description }}</p>
                </div>
                <div>
                    <font-awesome-icon
                        :icon="['fas', 'trash']"
                        :size="iconSize"
                        class="faicon red"
                        v-on:click="toggleShowDetails"
                    />
                    <font-awesome-icon
                        :icon="['fas', 'edit']"
                        :size="iconSize"
                        class="faicon green"
                        v-on:click="toggleShowDetails"
                    />
                </div>
            </div>
            <div v-if="showDetails">
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
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "PowerPlantCard",

    props: {
        powerPlant: Object,
    },

    data() {
        return {
            showDetails: false,
            iconSize: "1.5x",
        };
    },

    methods: {
        toggleShowDetails() {
            this.showDetails = !this.showDetails;
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
    margin: 1rem;
    width: 40vw;
}

p {
    padding-bottom: 0;
    /* padding-top: 0.25rem; */
    text-align: center;
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
    color: green;
}
</style>
