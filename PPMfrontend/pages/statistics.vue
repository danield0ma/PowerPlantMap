<template>
    <div class="content">
        <h1>Statisztikák</h1>
        <div>
            <!-- <h5>Ország választása</h5>
            <select v-model="selectedCountry">
                <option
                    v-for="country in countries"
                    :key="country.name"
                    :value="country"
                >
                    {{ country.name }}
                </option>
            </select> -->
        </div>
        <div class="d-flex justify-content-center">
            <h5 class="pr-3">Dátum választása</h5>
            <input
                type="date"
                v-model="chosenDate"
                @change="setDate"
                :min="minDate"
                :max="maxDate"
            />
        </div>

        <b-form-checkbox v-model="isCountrySelected" switch class="switch">
            Országok statisztikája
        </b-form-checkbox>

        <h3>
            Szeretnéd ezeket a statisztikákat minden reggel a postaládádban
            látni?
        </h3>
        <h5>Email feliratkozás:</h5>
        <input type="email" placeholder="E-mail cím" v-model="email" />
        <button class="btn btn-primary" v-on:click="addEmailSubscription">
            Feliratkozás
        </button>

        <div
            class="p-3 margin-auto d-flex align-items-center"
            v-if="isCountrySelected"
        >
            <table class="mx-auto d-block">
                <thead>
                    <tr>
                        <th class="p-2">Ország</th>
                        <th class="p-2">Importált energia [MWh]</th>
                        <th class="p-2">Exportált energia [MWh]</th>
                        <th class="p-2">Szaldó [MWh]</th>
                    </tr>
                </thead>
                <tbody>
                    <tr
                        v-for="country in this.countryStatistics.data"
                        :key="country.countryId"
                    >
                        <td class="p-2">{{ country.countryName }}</td>
                        <td class="p-2">{{ country.importedEnergy }}</td>
                        <td class="p-2">{{ country.exportedEnergy }}</td>
                        <td class="p-2">
                            {{
                                country.importedEnergy - country.exportedEnergy
                            }}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="p-3 grid" v-else>
            <div
                v-for="powerPlant in this.powerPlantStatistics.data"
                :key="powerPlant.generatorId"
            >
                <StatsCard :powerPlant="powerPlant"></StatsCard>
            </div>
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
            isCountrySelected: false,
            email: "",
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
            store.state.power.date = moment(Date.now())
                .subtract(1, "days")
                .format("YYYY-MM-DD");
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
table {
    border-collapse: collapse;
    padding: 2rem;
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

.switch .custom-control-label::before {
    background-color: #f8f9fa;
    border: 2px solid #6c757d;
}

.switch .custom-control-input:checked ~ .custom-control-label::before {
    background-color: #6c757d;
    border-color: #6c757d;
}
</style>
