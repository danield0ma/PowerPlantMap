<template>
    <div class="content">
        <h1>PowerPlantMap Statisztikák</h1>
        <h2>{{ chosenDate }}</h2>
        <div class="d-flex justify-content-center mt-3 mb-2">
            <h5 class="pr-3">Dátum választása</h5>
            <input
                type="date"
                v-model="chosenDate"
                @change="setDate"
                :min="minDate"
                :max="maxDate"
            />
        </div>

        <div class="d-flex justify-content-center">
            <p class="mr-2">Erőművek</p>
            <b-form-checkbox v-model="isCountrySelected" switch class="switch">
                <p>Országok</p>
            </b-form-checkbox>
        </div>

        <div
            class="p-3 margin-auto d-flex align-items-center table-container"
            v-if="isCountrySelected"
        >
            <table class="mx-auto d-block statsTable mx-auto d-block">
                <thead>
                    <tr>
                        <th class="p-3">Ország</th>
                        <th class="p-3">Importált energia [MWh]</th>
                        <th class="p-3">Exportált energia [MWh]</th>
                        <th class="p-3">Szaldó [MWh]</th>
                    </tr>
                </thead>
                <tbody>
                    <tr
                        v-for="country in this.countryStatistics.data"
                        :key="country.countryId"
                    >
                        <td class="p-3">{{ country.countryName }}</td>
                        <td class="p-3">{{ country.importedEnergy }}</td>
                        <td class="p-3">{{ country.exportedEnergy }}</td>
                        <td class="p-3">
                            {{
                                country.importedEnergy - country.exportedEnergy
                            }}
                        </td>
                    </tr>
                    <tr class="font-weight-bold">
                        <td class="p-3">Összesen</td>
                        <td class="p-3">
                            {{
                                this.countryStatistics.data.reduce(
                                    (a, b) => a + b.importedEnergy,
                                    0
                                )
                            }}
                        </td>
                        <td class="p-3">
                            {{
                                this.countryStatistics.data.reduce(
                                    (a, b) => a + b.exportedEnergy,
                                    0
                                )
                            }}
                        </td>
                        <td class="p-3">
                            {{
                                this.countryStatistics.data.reduce(
                                    (a, b) =>
                                        a + b.importedEnergy - b.exportedEnergy,
                                    0
                                )
                            }}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="p-3 grid" v-else>
            <div
                v-for="powerPlant in this.compactPowerPlantStatistics"
                :key="powerPlant.powerPlantId"
            >
                <StatsCard
                    :compactPowerPlant="powerPlant"
                    :powerPlant="
                        powerPlantStatistics.data.filter(
                            (x) => x.powerPlantId === powerPlant.powerPlantId
                        )
                    "
                    v-on:click="
                        selectedPowerPlant = powerPlant.powerPlantId;
                        showModal = true;
                    "
                ></StatsCard>
            </div>
        </div>

        <div class="m-5">
            <h3>
                Szeretnéd az aktuális statisztikákat minden reggel a
                postaládádban látni?
            </h3>
            <h4>Iratkozz fel!</h4>
            <input type="email" placeholder="E-mail cím" v-model="email" />
            <button class="btn btn-primary" v-on:click="addEmailSubscription">
                Feliratkozás
            </button>
        </div>
    </div>
</template>

<script>
import moment from "moment";
import StatsCard from "../components/Stats/StatsCard";

export default {
    name: "Statistics",

    head() {
        return {
            title: "Statistics - PowerPlantMap",
        };
    },

    components: { StatsCard },

    data() {
        return {
            compactPowerPlantStatistics: {},
            powerPlantStatistics: {},
            countryStatistics: {},
            countries: [
                { name: "Magyarország", img: "/hu.png" },
                { name: "Ausztria", img: "/austria.png" },
            ],
            selectedCountry: { name: "Magyarország", img: "/hu.png" },
            minDate: "2015-01-01",
            maxDate: moment(Date(Date.now())).format("YYYY-MM-DD"),
            isCountrySelected: false,
            email: "",

            selectedPowerPlant: "",
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
        let compactPowerPlantStatistics;
        let powerPlantStatistics;
        let countryStatistics;
        const date = store.state.power.date;
        if (
            date === null ||
            date === undefined ||
            date === moment(Date(Date.now())).format("YYYY-MM-DD")
        ) {
            store.state.power.date = moment(Date.now())
                .subtract(1, "days")
                .format("YYYY-MM-DD");
            compactPowerPlantStatistics = await $axios.$get(
                "/api/Statistics/GenerateCompactPowerPlantStatistics"
            );
            powerPlantStatistics = await $axios.$get(
                "/api/Statistics/GeneratePowerPlantStatistics"
            );
            countryStatistics = await $axios.$get(
                "/api/Statistics/GenerateCountryStatistics"
            );
        } else {
            compactPowerPlantStatistics = await $axios.$get(
                `/api/Statistics/GenerateCompactPowerPlantStatistics?day=${date}`
            );
            powerPlantStatistics = await $axios.$get(
                `/api/Statistics/GeneratePowerPlantStatistics?day=${date}`
            );
            countryStatistics = await $axios.$get(
                `/api/Statistics/GenerateCountryStatistics?day=${date}`
            );
        }
        return {
            compactPowerPlantStatistics,
            powerPlantStatistics,
            countryStatistics,
        };
    },

    methods: {
        async setDate() {
            if (this.chosenDate != null) {
                this.compactPowerPlantStatistics = await this.$axios.$get(
                    `/api/Statistics/GenerateCompactPowerPlantStatistics?day=${this.getDate}`
                );
                this.powerPlantStatistics = await this.$axios.$get(
                    `/api/Statistics/GeneratePowerPlantStatistics?day=${this.getDate}`
                );
                this.countryStatistics = await this.$axios.$get(
                    `/api/Statistics/GenerateCountryStatistics?day=${this.getDate}`
                );
            }
        },

        async addEmailSubscription() {
            try {
                await this.$axios.$post(
                    "/api/EmailSubscriptions/Add?email=" + this.email
                );
                alert("Sikeresen feliratkoztál!");
            } catch (error) {
                alert("Hibás e-mail cím!");
            }
            this.email = "";
        },
    },
};
</script>

<style scoped>
.switch .custom-control-label::before {
    background-color: #f8f9fa;
    border: 2px solid #6c757d;
}

.switch .custom-control-input:checked ~ .custom-control-label::before {
    background-color: #6c757d;
    border-color: #6c757d;
}

.table-container {
    display: flex;
    justify-content: center !important;
    align-items: center;
    width: 100%;
}

.statsTable {
    border-collapse: collapse;
    padding: 2rem;
    border-collapse: collapse;
    font-family: Arial, sans-serif;
    margin: auto;
}

.statsTable th,
.statsTable td {
    border: 1px solid #ddd;
    padding: 16px;
    text-align: center;
}

.statsTable th {
    padding: 12px;
    text-align: left;
    background-color: #4caf50;
    color: white;
}

.statsTable tr:nth-child(even) {
    background-color: #f2f2f2;
}

.statsTable tr:hover {
    background-color: #ddd;
}

.pointer {
    cursor: pointer;
}
</style>
