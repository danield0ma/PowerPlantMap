<template>
  <canvas ref="lineChart"></canvas>
</template>
  
<script>
import moment from "moment";
import { Chart } from "chart.js";

export default {
  data() {
    return {
      number: 123.456789,
    };
  },

  props: {
    blocId: {
      type: String,
      required: true,
    },
    generatorId: {
      type: String,
      required: true,
    },
    generator: {
      type: Boolean,
      required: true,
    },
  },

  mounted() {
    const ctx = this.$refs.lineChart.getContext("2d");
    new Chart(ctx, {
      type: "line",
      data: {
        labels: this.getDateArray(),
        datasets: [
          {
            label: "Power [MW]",
            backgroundColor: this.color,
            borderColor: this.color,
            fill: { value: 0 },
            data: this.getPowerArray(this.blocId, this.generator),
          }
        ]
      },
      options: {
        elements: {
          line: {
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
            text: "test",
            labels: {
              font: {
                size: 20,
              },
            },
          },
          legend: {
            display: false,
          },
          tooltip: {
            intersect: false,
            mode: "nearest",
          },
        },
        scales: {
          y: {
            min: this.getMin(),
            max: this.getMax(this.generatorID),
            grid: {
              lineWidth: 0,
            },
            stacked: false,
          },
          x: {
            grid: {
              lineWidth: 0,
            },
          },
        },
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      },
    });
  },

  computed: {
    formattedNumber() {
      return this.number.toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 4,
      });
    },

    isLoading() {
      return this.$store.state.power.isLoading;
    },

    content() {
      while (this.isLoading) {
        pass;
      }
      return this.$store.state.power.content;
    },
  },

  methods: {
    getDateArray() {
      let time = this.content.dataStart;
      let dateArray = [];
      dateArray.push(moment(time).format("HH:mm"));
      for (let i = 0; i < 97; i++) {
        time = moment(time).add(15, "m");
        dateArray.push(moment(time).format("HH:mm"));
      }
      return dateArray;
    },

    getPowerArray(blocId, generator) {
      let choice = false;
      if (blocId == "all") {
        choice = true;
      }

      let data = this.content;
      let a = [];
      for (let i = 0; i < 97; i++) {
        a.push(0);
      }

      for (let bloc of data.blocs) {
        if (choice || blocId == bloc.blocID) {
          for (let generator of bloc.generators) {
            for (let i = 0; i < 96; i++) {
              a[i] += generator.pastPower[i].power;
            }
          }
        }
      }

      if (generator) {
        for (let bloc of data.blocs) {
          for (let generator of bloc.generators) {
            if (blocId == generator.generatorID) {
              for (let i = 0; i < 96; i++) {
                a[i] += generator.pastPower[i].power;
              }
            }
          }
        }
      }

      console.log(a);
      return a;
    },

    getMin() {
      if (this.content.isCountry) {
        let array = this.content.blocs[0].generators[0].pastPower;
        let min = Math.min(...array);
        return Math.floor(min / 100) * 100;
      } else {
        return 0;
      }
    },

    getMax(generatorID) {
      let selectedBloc = this.$store.state.power.selectedBloc;
      let array = [];
      for (let i = 0; i < 100; i++) {
        array.push(0);
      }

      if (selectedBloc === -1) {
        for (let bloc of this.content.blocs) {
          for (let generator of bloc.generators) {
            for (let i = 0; i < generator.pastPower.length; i++) {
              array[i] += generator.pastPower[i];
            }
          }
        }
      } else {
        for (let generator of this.content.blocs[selectedBloc].generators) {
          if (generatorID == "" || generatorID == generator.generatorID) {
            for (let i = 0; i < generator.pastPower.length; i++) {
              array[i] += generator.pastPower[i];
            }
          }
        }
      }

      let sum = 0;
      for (let i = 0; i < 100; i++) {
        sum += array[i];
      }
      if (sum == 0) {
        return 100;
      }

      let max = Math.max(...array);
      return Math.ceil(max / 100) * 100;
    },
  },
};
</script>