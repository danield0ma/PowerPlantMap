<template>
    <div class="Admin">
        <div class="container">
            <div class="Card" style="flex-direction: column">
                <h5>Ország választása</h5>
                <select v-model="selectedCountry">
                    <option
                        v-for="country in countries"
                        :key="country.name"
                        :value="country"
                    >
                        {{ country.name }}
                    </option>
                </select>
                <!-- <div class="dropdown">
                    <button @click="isOpen = !isOpen">
                        {{ selectedCountry || "Select a country" }}
                    </button>
                    <ul v-if="isOpen">
                        <li
                            v-for="country in countries"
                            :key="country.name"
                            @click="selectCountry(country)"
                        >
                            <img :src="country.flag" :alt="country.name" />
                            {{ country.name }}
                        </li>
                    </ul>
                </div> -->
            </div>
            <div class="Card">
                <h5>Új erőmű létrehozása</h5>
                <font-awesome-icon
                    :icon="['fas', 'plus']"
                    :size="'2x'"
                    class="faicon green"
                    v-on:click="toggleAddNewPowerPlant"
                />
            </div>
        </div>
        <AddPowerPlant
            :powerPlant="{}"
            v-if="showModal"
            @close="showModal = false"
        />
        <h1 v-on:click="toggleShowPowerPlants">Erőművek</h1>
        <transition name="fade" mode="out-in">
            <div class="grid" v-if="showPowerPlants">
                <div
                    v-for="currentPowerPlant in FilteredPowerPlants"
                    :key="currentPowerPlant.powerPlantId"
                >
                    <PowerPlantCard
                        :powerPlant="currentPowerPlant"
                    ></PowerPlantCard>
                </div>
            </div>
        </transition>
        <h1 v-on:click="toggleShowCountries">Szomszédos országok</h1>
        <transition name="fade" mode="out-in">
            <div class="grid" v-if="showCountries">
                <div
                    v-for="currentPowerPlant in FilteredCountries"
                    :key="currentPowerPlant.powerPlantId"
                >
                    <PowerPlantCard
                        :powerPlant="currentPowerPlant"
                    ></PowerPlantCard>
                </div>
            </div>
        </transition>
        <h1 v-on:click="toggleShowUnknownPowerPlants">Energiaforrás típusok</h1>
        <transition name="fade" mode="out-in">
            <div class="grid" v-if="showUnknownPowerPlants">
                <div
                    v-for="currentPowerPlant in FilteredUnknownPowerPlants"
                    :key="currentPowerPlant.powerPlantId"
                >
                    <PowerPlantCard
                        :powerPlant="currentPowerPlant"
                    ></PowerPlantCard>
                </div>
            </div>
        </transition>
    </div>
</template>

<script>
import PowerPlantCard from "../../components/PowerPlants/PowerPlantCard";
import PowerPlantModal from "../../components/PowerPlants/PowerPlantModal";
export default {
    name: "PowerPlantEditor",
    layout: "adminLayout",
    components: { PowerPlantCard, AddPowerPlant: PowerPlantModal },
    head() {
        return {
            title: "PowerPlant Editor - PowerPlantMap",
        };
    },

    data() {
        return {
            powerPlants: [],
            BASE_PATH: "https://powerplantmap.tech:5001/",
            showModal: false,
            showPowerPlants: true,
            showCountries: true,
            showUnknownPowerPlants: true,

            countries: [
                { name: "Magyarország", img: "/hu.png" },
                { name: "Ausztria", img: "/austria.png" },
            ],
            isOpen: true,
            selectedCountry: { name: "Magyarország", img: "/hu.png" },
        };
    },

    async asyncData() {
        const BASE_PATH = "https://powerplantmap.tech:5001/";
        const res = await fetch(`${BASE_PATH}API/PowerPlant/Get`);
        const powerPlants = await res.json();
        return { powerPlants };
    },

    computed: {
        FilteredCountries() {
            return this.powerPlants.filter(
                (powerPlant) => powerPlant.isCountry === true
            );
        },

        FilteredPowerPlants() {
            return this.powerPlants.filter(
                (powerPlant) =>
                    powerPlant.isCountry === false && powerPlant.address !== "-"
            );
        },

        FilteredUnknownPowerPlants() {
            return this.powerPlants.filter(
                (powerPlant) =>
                    powerPlant.isCountry === false && powerPlant.address === "-"
            );
        },
    },

    methods: {
        selectCountry(country) {
            this.selectedCountry = country.name;
            this.isOpen = false;
        },

        toggleAddNewPowerPlant() {
            this.showModal = !this.showModal;
        },

        toggleShowPowerPlants() {
            this.showPowerPlants = !this.showPowerPlants;
        },

        toggleShowCountries() {
            this.showCountries = !this.showCountries;
        },

        toggleShowUnknownPowerPlants() {
            this.showUnknownPowerPlants = !this.showUnknownPowerPlants;
        },
    },
};
</script>

<style scoped>
body {
    margin: 0;
    padding: 0;
}

.Admin {
    background-color: white;
    overflow: auto;
    max-height: calc(100vh - 3.5rem);
    margin-top: 3.5rem;
    height: calc(100vh - 3.5rem);
    justify-content: center;
}

.Modal {
    background-color: #808080;
    overflow: auto;
    max-height: calc(100vh - 3.5rem);
    margin-top: 3.5rem;
    height: calc(100vh - 3.5rem);
    justify-content: center;
}

.container {
    display: flex;
    justify-content: space-between;
    padding: 0 20vw;
    text-align: center;
}

.Card {
    background-color: white;
    border: 1px solid black;
    border-radius: 5px;
    padding: 1rem;
    margin: 1rem;
    width: 250px;
    display: flex;
    justify-content: space-around;
    min-height: 100px;
    box-shadow: 0 5px 5px rgba(0, 0, 0, 0.25);
}

.Card:hover {
    background-color: lightgray;
}

p {
    padding-bottom: 0;
    /* padding-top: 0.25rem; */
    text-align: center;
}

.grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    flex-direction: column;
    justify-content: center;
    align-items: center;
    gap: 15px;
    padding: 0 1rem 1rem 1rem;
}

h1 {
    padding: 0 1rem;
    cursor: pointer;
}

h5 {
    padding: 0;
}

@keyframes slide-up {
    0% {
        transform: translateY(calc(100% + 1em));
        opacity: 0;
    }
    100% {
        transform: translateY(0);
        opacity: 1;
    }
}

.fade-enter-active {
    animation: slide-up 0.5s forwards;
}
.fade-leave-active {
    animation: slide-up 0.5s reverse forwards;
}

.dropdown {
    position: relative;
    display: inline-block;
}

.dropdown button {
    background-color: #4caf50;
    color: white;
    padding: 10px;
    font-size: 16px;
    border: none;
    cursor: pointer;
    border-radius: 10px;
    margin-top: 0.5rem;
    z-index: 10;
}

.dropdown button:hover {
    background-color: #45a049;
}

.dropdown ul {
    display: none;
    position: absolute;
    background-color: #f9f9f9;
    min-width: 160px;
    box-shadow: 0px 8px 16px 0px rgba(0, 0, 0, 0.2);
    z-index: 1;
}

.dropdown li {
    color: black;
    padding: 12px 16px;
    text-decoration: none;
    display: block;
}

.dropdown li:hover {
    background-color: #f1f1f1;
}

.dropdown img {
    width: 20px;
    height: 20px;
    margin-right: 10px;
}

.dropdown ul.show {
    display: block;
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
