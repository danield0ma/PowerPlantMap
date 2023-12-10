<template>
    <div id="innerRight">
        <div v-if="isRightPanelLoading">
            <h1>Loading...</h1>
        </div>
        <div v-else>
            <div class="d-flex justify-content-between m-2">
                <div class="col-md-7 p-0">
                    <div class="d-flex">
                        <div style="display: inline; vertical-align: sub">
                            <img
                                src="hu.png"
                                alt="zászló"
                                width="40px"
                                height="20px"
                                style="margin-top: 0.7rem"
                            />
                        </div>
                        <h4
                            style="
                                padding-left: 0.5rem;
                                display: inline;
                                vertical-align: top;
                                margin: 0.25rem 0;
                            "
                        >
                            Magyarország
                        </h4>
                    </div>
                    <p class="p-0 text-left">{{ startTime }} - {{ endTime }}</p>
                </div>
                <div class="col-md-5 p-0 d-flex flex-column">
                    <div class="ml-auto">
                        <font-awesome-icon
                            icon="fa-solid fa-xmark"
                            class="faicon red"
                            v-on:click="closeRightPanel"
                            :size="'lg'"
                        />
                    </div>
                    <input
                        type="date"
                        v-model="chosenDate"
                        @change="$emit('changeDate')"
                        :min="minDate"
                        :max="maxDate"
                        class="dateInput"
                    />
                </div>
            </div>
            <div>
                <client-only>
                    <line-chart
                        :chart-data="chartData()"
                        :chart-options="chartOptions"
                        :height="500"
                        :width="500"
                        chart-id="Energiamix"
                    />
                </client-only>
            </div>
        </div>
    </div>
</template>

<script>
import moment from "moment";
import "chart.js";

export default {
    name: "Energymix",

    props: {
        powerOfPowerPlants: {
            type: Object,
            required: true,
        },
        basicsOfPowerPlants: {
            type: Array,
            required: true,
        },
    },

    data() {
        return {
            minDate: "2015-01-01",
            maxDate: moment(Date(Date.now())).format("YYYY-MM-DD"),
        };
    },

    computed: {
        isRightPanelLoading() {
            return this.$store.state.power.rightLoading;
        },

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

        startTime() {
            return moment(this.powerOfPowerPlants.start).format(
                "YYYY.MM.DD HH:mm"
            );
        },

        endTime() {
            return moment(this.powerOfPowerPlants.end).format(
                "YYYY.MM.DD HH:mm"
            );
        },

        chartOptions() {
            return {
                elements: {
                    line: {
                        borderColor: "#C1536D",
                        borderWidth: 3,
                    },
                    point: {
                        pointRadius: 0,
                    },
                },
                layout: { padding: 0 },
                tooltips: { enabled: true },
                plugins: {
                    title: {
                        display: true,
                        text: "Energiamix diagram",
                    },
                    legend: {
                        display: false,
                    },
                    tooltip: {
                        intersect: false,
                    },
                },
                scales: {
                    y: {
                        grid: {
                            lineWidth: 0,
                        },
                        stacked: true,
                    },
                    x: {
                        grid: {
                            lineWidth: 0,
                        },
                    },
                },
                minimumFractionDigits: 2,
            };
        },
    },

    methods: {
        closeRightPanel() {
            this.$store.dispatch("power/setRightPanel", false);
        },

        chartData() {
            const createdDatasets = [];
            for (const [
                index,
                basicsOfPowerPlant,
            ] of this.basicsOfPowerPlants.entries()) {
                let fill = "-1";
                if (index === 0) {
                    fill = { value: 0 };
                }

                createdDatasets.push({
                    label: basicsOfPowerPlant.description + " [MW]",
                    backgroundColor: "#" + basicsOfPowerPlant.color,
                    borderColor: "#" + basicsOfPowerPlant.color,
                    pointRadius: 0,
                    stack: "PP",
                    fill: fill,
                    data: this.getPowerOfPowerPlant(basicsOfPowerPlant.id),
                });
            }

            return {
                labels: this.getDateArray("PKS"),
                datasets: createdDatasets,
            };
        },

        getDateArray(PowerPlantId) {
            return this.powerOfPowerPlants.data
                .filter((x) => x.powerPlantName === PowerPlantId)
                .flatMap((x) =>
                    x.powerStamps.map((y) => moment(y.start).format("HH:mm"))
                );
        },

        getPowerOfPowerPlant(powerPlantId) {
            return this.powerOfPowerPlants.data
                .filter((x) => x.powerPlantName === powerPlantId)
                .flatMap((x) => x.powerStamps.map((y) => y.power));
        },
    },
};
</script>

<style scoped>
#innerRight {
    max-height: calc(100vh - 3.5rem);
    overflow: auto;
}

#dateInput {
    max-width: 200px !important;
    margin-left: auto;
}
</style>
