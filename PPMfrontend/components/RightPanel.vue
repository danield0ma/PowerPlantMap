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
        <h4 style="padding-left: 0.5rem; display: inline; vertical-align: top">
          Magyarország
        </h4>
      </div>
      <p style="padding: 0">{{ startTime }} - {{ endTime }}</p>
      <!-- <h6>Teljes rendszerterhelés: {{ this.$store.state.power.currentLoad }} MW</h6>
            <h6>Energia-mix diagram</h6> -->
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
      <!-- <TotalChart
        blocId="Gönyű 1"
        generatorId="Gönyű 1"
        v-bind:generator="false"
      /> -->
    </div>
  </div>
</template>

<script>
import moment from "moment";
import "chart.js";
import TotalChart from "~/components/TotalChart.vue";

export default {
  computed: {
    startTime() {
      return moment(this.$store.state.power.powerOfPowerPlants.start).format(
        "YYYY.MM.DD HH:mm"
      );
    },

    endTime() {
      return moment(
        this.$store.state.power.powerOfPowerPlants.end
      )./*add(-15, 'm').*/ format("YYYY.MM.DD HH:mm");
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
        labels: this.getDateArray(),
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

    getDateArray() {
      let time = this.startTime;
      let dateArray = [];
      dateArray.push(moment(time).format("HH:mm"));
      for (let i = 0; i < 97; i++) {
        time = moment(time).add(15, "m");
        dateArray.push(moment(time).format("HH:mm"));
      }
      return dateArray;
    },

    getLoadArray() {
      let loadArray = [];
      for (const load of this.$store.state.power.loadHistory) {
        loadArray.push(load.currentLoad);
      }

      return loadArray;
    },

    getPowerOfPowerPlant(PPID) {
      let powerArray = this.$store.state.power.powerOfPowerPlants.data;
      for (let power of powerArray) {
        if (power.powerPlantName == PPID) {
          //return power.powerStamps
          let powerData = [];
          for (let powerStamp of power.powerStamps) {
            powerData.push(powerStamp.power);
          }
          return powerData;
        }
      }
    },

    getPowerOfGAS() {
      let gasPPs = ["DME", "GNY", "CSP", "KF", "KP"];
      let power = [];
      for (let i = 0; i < 100; i++) {
        power.push(0);
      }

      for (let gasPP of gasPPs) {
        let array = this.getPowerOfPowerPlant(gasPP);
        for (let i = 0; i < array.length; i++) {
          power[i] += array[i];
        }
      }

      let GASarray = this.getPowerOfPowerPlant("GAS");

      let result = [];
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