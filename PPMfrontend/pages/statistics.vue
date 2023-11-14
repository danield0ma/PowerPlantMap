<template>
    <div class="content">
        <h1>Statistics</h1>
        <div>
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
        </div>
        <input
            type="date"
            v-model="chosenDate"
            @change="setDate"
            :min="minDate"
            :max="maxDate"
        />
        <p>{{ this.powerPlantStatistics.start }}</p>
        <div class="d-flex justify-content-between align-center">
            <div class="col-md-8 p-3">
                <div
                    v-for="powerPlant in this.powerPlantStatistics.data"
                    :key="powerPlant.generatorId"
                    class="grid"
                >
                    <div class="Card">
                        <p>{{ powerPlant.powerPlantId }}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4 p-3">
                <table class="mx-auto d-block">
                    <thead>
                        <tr>
                            <th class="p-2">Ország</th>
                            <th class="p-2">Importált energia</th>
                            <th class="p-2">Exportált energia</th>
                            <th class="p-2">Szaldó</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr
                            v-for="country in this.countryStatistics.data"
                            :key="country.countryId"
                        >
                            <td class="p-2">{{ country.countryId }}</td>
                            <td class="p-2">{{ country.importedEnergy }}</td>
                            <td class="p-2">{{ country.exportedEnergy }}</td>
                            <td class="p-2">
                                {{
                                    country.importedEnergy -
                                    country.exportedEnergy
                                }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<script>
import moment from "moment";

export default {
    name: "Statistics",

    head() {
        return {
            title: "Statistics - PowerPlantMap",
        };
    },

    data() {
        return {
            powerPlantStatistics: {},
            countryStatistics: {},
            countries: [
                { name: "Magyarország", img: "/hu.png" },
                { name: "Ausztria", img: "/austria.png" },
            ],
            selectedCountry: { name: "Magyarország", img: "/hu.png" },
            minDate: "2015-01-01",
            maxDate: moment(Date(Date.now())).format("YYYY-MM-DD"),
        };
    },

    mounted() {
        if (this.getDate === null || this.getDate === undefined) {
            this.$store.dispatch(
                "power/setDate",
                moment(Date(Date.now())).format("YYYY-MM-DD")
            );
        }
    },

    computed: {
        getDate() {
            return this.$store.state.power.date;
        },

        chosenDate: {
            get() {
                return this.$store.state.power.date;
            },
            set(value) {
                this.$store.dispatch("power/setDate", value);
            },
        },
    },

    async asyncData({ $axios, store }) {
        let powerPlantStatistics;
        let countryStatistics;
        const date = store.state.power.date;
        if (
            date === null ||
            date === undefined ||
            date === moment(Date(Date.now())).format("YYYY-MM-DD")
        ) {
            powerPlantStatistics = await $axios.$get(
                "/api/Statistics/GeneratePowerPlantStatistics"
            );
            countryStatistics = await $axios.$get(
                "/api/Statistics/GenerateCountryStatistics"
            );
        } else {
            powerPlantStatistics = await $axios.$get(
                `/api/Statistics/GeneratePowerPlantStatistics?day=${date}`
            );
            countryStatistics = await $axios.$get(
                `/api/Statistics/GenerateCountryStatistics?day=${date}`
            );
        }
        return { powerPlantStatistics, countryStatistics };
    },

    methods: {
        async setDate() {
            if (this.chosenDate != null) {
                this.powerPlantStatistics = await this.$axios.$get(
                    `/api/Statistics/GeneratePowerPlantStatistics?day=${this.getDate}`
                );
                this.countryStatistics = await this.$axios.$get(
                    `/api/Statistics/GenerateCountryStatistics?day=${this.getDate}`
                );
            }
        },
    },
};
</script>

<style>
body {
    margin: 0;
    padding: 0;
}

table {
    border-collapse: collapse;
    padding: 2rem;
}

.content {
    background-color: white;
    overflow-y: auto;
    padding-top: 4rem;
    text-align: center;
    margin: auto;
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

.grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    flex-direction: column;
    justify-content: center;
    align-items: center;
    gap: 15px;
    padding: 0 1rem 1rem 1rem;
}
</style>
