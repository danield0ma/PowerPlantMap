<template>
    <div>
        <div id="innerRight">
            <div style="display: flex">
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
                    "
                >
                    Hungary
                </h4>
            </div>
            <p style="padding: 0">{{ startTime }} - {{ endTime }}</p>
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
        powerArray: {
            type: Object,
            required: true,
        },
    },

    computed: {
        startTime() {
            return moment(this.powerArray.start).format("YYYY.MM.DD HH:mm");
        },

        endTime() {
            return moment(this.powerArray.end)./*add(-15, 'm').*/ format(
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
                layout: {
                    padding: 0,
                },
                tooltips: {
                    enabled: true,
                },
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
                        //min: -2500,
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
        chartData() {
            return {
                labels: this.getDateArray("PKS"),
                datasets: [
                    // {
                    //     label: 'Total System Load [MW]',
                    //     backgroundColor: '#777',
                    //     borderColor: '#777',
                    //     fill: false,
                    //     data: this.getLoadArray()
                    // },
                    {
                        label: "Paks [MW]",
                        backgroundColor: "#B7BF50",
                        borderColor: "#B7BF50",
                        pointRadius: 0,
                        stack: "PP",
                        //fill: origin,
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
                        data: this.getPowerOfGAS(),
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

        getDateArray(PPID) {
            const powerOfPowerPlants = JSON.parse(
                JSON.stringify(this.powerArray)
            );
            const powerData = powerOfPowerPlants.data
                .filter((x) => x.powerPlantName === PPID)
                .flatMap((x) =>
                    x.powerStamps.map((y) => moment(y.start).format("HH:mm"))
                );
            return powerData;
        },

        getLoadArray() {
            const loadArray = [];
            for (const load of this.$store.state.power.loadHistory) {
                loadArray.push(load.currentLoad);
            }

            return loadArray;
        },

        getPowerOfPowerPlant(PPID) {
            const powerOfPowerPlants = JSON.parse(
                JSON.stringify(this.powerArray)
            );
            const powerData = powerOfPowerPlants.data
                .filter((x) => x.powerPlantName === PPID)
                .flatMap((x) => x.powerStamps.map((y) => y.power));
            return powerData;
        },

        getPowerOfGAS() {
            const gasPowerPlants = ["DME", "GNY", "CSP", "KF", "KP"];
            const power = [];
            for (let i = 0; i < 100; i++) {
                power.push(0);
            }

            for (const gasPP of gasPowerPlants) {
                const array = this.getPowerOfPowerPlant(gasPP);
                for (let i = 0; i < array.length; i++) {
                    power[i] += array[i];
                }
            }

            const GASarray = this.getPowerOfPowerPlant("GAS");

            const result = [];
            for (let i = 0; i < GASarray.length; i++) {
                result[i] = GASarray[i] - power[i];
            }

            return result;
        },
    },
};
</script>

<style>
#innerRight {
    max-height: calc(100vh - 3.5rem);
    overflow: auto;
}
</style>