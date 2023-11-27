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
            return {
                labels: this.getDateArray("PKS"),
                datasets: [
                    {
                        label: "Paks [MW]",
                        backgroundColor: "#B7BF50",
                        borderColor: "#B7BF50",
                        pointRadius: 0,
                        stack: "PP",
                        fill: { value: 0 },
                        data: this.getPowerOfPowerPlant("PKS"),
                    },
                    {
                        label: "Mátra [MW]",
                        backgroundColor: "#B59C5E",
                        borderColor: "#B59C5E",
                        pointRadius: 0,
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("MTR"),
                    },
                    {
                        label: "Biomassza (ismeretlen erőművekből) [MW]",
                        backgroundColor: "#3E8172",
                        borderColor: "#3E8172",
                        pointRadius: 0,
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("BIO"),
                    },
                    {
                        label: "Dunamenti [MW]",
                        backgroundColor: "#e691a5",
                        borderColor: "#e691a5",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("DME"),
                    },
                    {
                        label: "Gönyű [MW]",
                        backgroundColor: "#C1536D",
                        borderColor: "#C1536D",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("GNY"),
                    },
                    {
                        label: "Csepel II. [MW]",
                        backgroundColor: "#990f30",
                        borderColor: "#990f30",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("CSP"),
                    },
                    {
                        label: "Kispest [MW]",
                        backgroundColor: "#5c0318",
                        borderColor: "#5c0318",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("KP"),
                    },
                    {
                        label: "Kelenföld [MW]",
                        backgroundColor: "#e691a5",
                        borderColor: "#e691a5",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("KF"),
                    },
                    {
                        label: "Ismeretlen gáz [MW]",
                        backgroundColor: "#C1536D",
                        borderColor: "#C1536D",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfGasPowerPlants(),
                    },
                    {
                        label: "Nap (ismeretlen erőművekből) [MW]",
                        backgroundColor: "#EE8931",
                        borderColor: "#EE8931",
                        pointRadius: 0,
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("SOL"),
                    },
                    {
                        label: "Szél (ismeretlen erőművekből) [MW]",
                        backgroundColor: "#89D0C0",
                        borderColor: "#89D0C0",
                        pointRadius: 0,
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("WND"),
                    },
                    {
                        label: "Litér [MW]",
                        backgroundColor: "#9D9684",
                        borderColor: "#9D9684",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("LIT"),
                    },
                    {
                        label: "Lőrinci [MW]",
                        backgroundColor: "#9D9684",
                        borderColor: "#9D9684",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("LOR"),
                    },
                    {
                        label: "Sajószöged [MW]",
                        backgroundColor: "#9D9684",
                        borderColor: "#9D9684",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("SAJ"),
                    },
                    {
                        label: "Szlovákia [MW]",
                        backgroundColor: "#0052B4",
                        borderColor: "#0052B4",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("SVK"),
                    },
                    {
                        label: "Ausztria [MW]",
                        backgroundColor: "#D80027",
                        borderColor: "#D80027",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("AUT"),
                    },
                    {
                        label: "Szlovénia [MW]",
                        backgroundColor: "#008B1B",
                        borderColor: "#008B1B",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("SLO"),
                    },
                    {
                        label: "Horvátország [MW]",
                        backgroundColor: "#171796",
                        borderColor: "#171796",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("CRO"),
                    },
                    {
                        label: "Szerbia [MW]",
                        backgroundColor: "#000",
                        borderColor: "#000",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("SRB"),
                    },
                    {
                        label: "Románia [MW]",
                        backgroundColor: "#BF9F11",
                        borderColor: "#BF9F11",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("ROU"),
                    },
                    {
                        label: "Ukrajna [MW]",
                        backgroundColor: "#FFDA44",
                        borderColor: "#FFDA44",
                        stack: "PP",
                        fill: "-1",
                        data: this.getPowerOfPowerPlant("UKR"),
                    },
                ],
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

        getPowerOfGasPowerPlants() {
            const gasPowerPlants = ["DME", "GNY", "CSP", "KF", "KP"];
            const power = [];
            for (let i = 0; i < 100; i++) {
                power.push(0);
            }

            for (const gasPowerPlant of gasPowerPlants) {
                const array = this.getPowerOfPowerPlant(gasPowerPlant);
                for (let i = 0; i < array.length; i++) {
                    power[i] += array[i];
                }
            }

            const gasArray = this.getPowerOfPowerPlant("GAS");

            const result = [];
            for (let i = 0; i < gasArray.length; i++) {
                result[i] = gasArray[i] - power[i];
            }

            return result;
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
