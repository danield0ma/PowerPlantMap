<template>
  <div>
    <div v-if="isLoading">
      <h1>LOADING...</h1>
    </div>
    <div v-else id="velse">
      <div class="flexbox">
        <h4>{{ content.description }}</h4>
        <div class="inline">
          <font-awesome-icon
            icon="fa-solid fa-xmark fa-xs"
            class="faicon"
            v-on:click="closePanel"
          />
        </div>
      </div>
      <p style="padding: 0">{{ dataStart }} - {{ dataEnd }}</p>
      <h6>Cím: {{ content.address }}</h6>
      <h6>GPS-koordináták: {{ content.longitude }}, {{ content.latitude }}</h6>
      <h6>Üzemeltető: {{ content.operatorCompany }}</h6>
      <h6>
        Weboldal:
        <a :href="content.webpage" target="_blank">{{ content.webpage }}</a>
      </h6>
      <!-- <a href={{ content.webpage }}>{{ content.webpage }}</a> -->
      <h6>Max teljesítmény: {{ content.maxPower }} MW</h6>

      <div v-if="blocsNotEnabled">
        <client-only>
          <line-chart
            :chart-data="chartData('all', false)"
            :chart-options="
              chartOptions('', content.description + ' termelése')
            "
            :height="300"
            :width="500"
            chart-id="Teljes"
          />
        </client-only>
      </div>

      <div class="flexbox" v-if="content.blocs.length > 1">
        <h4>Blokkok</h4>
        <div class="inline">
          <div v-if="blocsEnabled">
            <font-awesome-icon
              icon="fa-solid fa-minus fa-xl"
              class="faicon"
              v-on:click="toggleBlocs"
            />
          </div>
          <div v-else>
            <font-awesome-icon
              icon="fa-solid fa-plus fa-xl"
              class="faicon"
              v-on:click="toggleBlocs"
              style="color: green"
            />
          </div>
        </div>
      </div>

      <div v-if="blocsEnabled">
        <client-only>
          <line-chart
            :chart-data="chartData(content.blocs[selectedBloc].blocID, false)"
            :chart-options="
              chartOptions('', content.blocs[selectedBloc].blocID)
            "
            :height="300"
            :width="500"
            chart-id="bloc"
          />
        </client-only>

        <div
          class="flexbox"
          style="padding: 0 5rem; justify-content: space-evenly"
        >
          <div v-for="(bloc, index) in content.blocs" :key="bloc.blocID">
            <button class="blocSelectionButton" v-on:click="selectBloc(index)">
              {{ /*bloc.blocID[bloc.blocID.length - 1]*/ index + 1 }}
            </button>
          </div>
        </div>

        <div v-if="content.blocs[selectedBloc].generators.length > 1">
          <!-- <div v-for="generator in content.blocs[selectedBloc].generators" :key="generator.generatorID">
                        <p>{{generator.generatorID}}: {{generator.pastPower[0]}}/{{generator.maxCapacity}}MW</p>
                    </div> -->
          <div class="flexbox" style="justify-content: space-around">
            <div>
              <line-chart
                :chart-data="
                  chartData(
                    content.blocs[selectedBloc].generators[0].generatorID,
                    true
                  )
                "
                :chart-options="
                  chartOptions(
                    content.blocs[selectedBloc].generators[0].generatorID,
                    content.blocs[selectedBloc].generators[0].generatorID
                  )
                "
                :height="150"
                :width="200"
                chart-id="bloc"
              />
            </div>
            <div>
              <line-chart
                :chart-data="
                  chartData(
                    content.blocs[selectedBloc].generators[1].generatorID,
                    true
                  )
                "
                :chart-options="
                  chartOptions(
                    content.blocs[selectedBloc].generators[1].generatorID,
                    content.blocs[selectedBloc].generators[1].generatorID
                  )
                "
                :height="150"
                :width="200"
                chart-id="bloc"
              />
            </div>
          </div>
        </div>

        <!-- <div v-for="bloc in content.blocs" :key="bloc.blocID">
                    <div class="flexbox">
                        <h6>{{bloc.blocID}} ({{bloc.blocType}}): {{bloc.pastPower}}/{{bloc.maxBlocCapacity}}MW</h6>
                        <div class="inline" v-if="bloc.generators.length > 1">
                            <font-awesome-icon icon="fa-solid fa-xmark fa-xs" class="faicon" />
                        </div>
                    </div>

                    <client-only>
                        <line-chart
                            :chart-data = "chartData(bloc.blocID)"
                            :chart-options = "chartOptions"
                            :height = "300"
                            :width = "500"
                            chart-id = bloc.blocID
                        />
                    </client-only>

                    <div v-if="bloc.generators.length > 1">
                        <div v-for="generator in bloc.generators" :key="generator.generatorID">
                            <p>{{generator.generatorID}}: {{generator.pastPower[0]}}/{{generator.maxCapacity}}MW</p>
                        </div>
                    </div>
                </div> -->
      </div>
    </div>
  </div>
</template>

<script>
import moment from "moment";

export default {
  name: "LeftPanel",

  // created() {
  //     moment.locale('hu')
  // },

  // data() {
  //     return {
  //         selectedBloc: 0
  //     }
  // },

  computed: {
    dataEnd() {
      return moment(
        this.$store.state.power.content.dataEnd
      )./*add(-15, 'm').*/ format("YYYY.MM.DD HH:mm");
    },

    dataStart() {
      return moment(this.$store.state.power.content.dataStart).format(
        "YYYY.MM.DD HH:mm"
      );
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

    blocsEnabled() {
      return this.$store.state.power.enableBlocs;
    },

    blocsNotEnabled() {
      return !this.$store.state.power.enableBlocs;
    },

    color() {
      return "#" + this.content.color;
    },

    selectedBloc() {
      return this.$store.state.power.selectedBloc;
    },
  },
  methods: {
    chartOptions(generatorID, title) {
      return {
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
            text: title,
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
            max: this.getMax(generatorID),
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
        minimumFractionDigits: 2,
      };
    },

    chartData(blocID, generator) {
      let asd = {
        labels: this.getDateArray(),
        datasets: [
          {
            label: "Power [MW]",
            backgroundColor: this.color,
            borderColor: this.color,
            fill: { value: 0 },
            data: this.getPowerArray(blocID, generator),
          },
          {
            label: "Max Capacity [MW]",
            backgroundColor: "#777",
            borderColor: "#777",
            fill: false,
            data: this.getMaxCap(blocID),
          },
        ],
      };
      return asd;
    },

    getContent() {
      return this.$store.state.power.content;
    },

    closePanel() {
      this.$store.dispatch("power/setLeftPanel", false);
    },

    getDateArray() {
      let time = this.content.dataStart;
      let dateArray = [];
      dateArray.push(moment(time).format("HH:mm"));
      for (let i = 0; i < 97; i++) {
        time = moment(time).add(15, "m");
        dateArray.push(moment(time).format("HH:mm"));
      }

      return dateArray;

      // let time = moment(this.$store.state.power.content.dataStart).add(15, 'm').toDate()

      // let timeArray = []
      // let resultArray = []
      // let previous = time
      // timeArray.push(time)
      // resultArray.push(moment(time).format("HH:mm"))
      // for(let i=1; i<97; i++)
      // {
      //     let time = moment(previous).add(15, 'm').toDate()
      //     timeArray.push(time)
      //     resultArray.push(moment(time).format("HH:mm"))
      //     previous = time
      // }
      // return resultArray
    },

    getPowerArray(blocID, generator) {
      let choice = false;
      if (blocID == "all") {
        choice = true;
      }

      let data = this.content;
      let a = [];
      for (let i = 0; i < 97; i++) {
        a.push(0);
      }

      for (let bloc of data.blocs) {
        if (choice || blocID == bloc.blocID) {
          for (let generator of bloc.generators) {
            for (let i = 0; i < 97; i++) {
              a[i] += generator.pastPower[i];
            }
          }
        }
      }

      if (generator) {
        for (let bloc of data.blocs) {
          for (let generator of bloc.generators) {
            if (blocID == generator.generatorID) {
              for (let i = 0; i < 97; i++) {
                a[i] += generator.pastPower[i];
              }
            }
          }
        }
      }

      return a;
    },

    getMaxCap(blocID) {
      let choice = false;
      if (blocID == "all") {
        choice = true;
      }
      let maxCap = 0;

      for (let bloc of this.content.blocs) {
        if (choice || blocID == bloc.blocID) {
          for (let generator of bloc.generators) {
            maxCap += generator.maxCapacity;
          }
        }
      }

      let arr = [];
      for (let i = 0; i < 97; i++) {
        arr.push(maxCap);
      }
      return arr;
    },

    async toggleBlocs() {
      if (this.blocsEnabled) {
        await this.$store.dispatch("power/toggleBlocs", false);
        await this.$store.dispatch("power/setSelectedBloc", -1);
      } else {
        await this.$store.dispatch("power/setSelectedBloc", 0);
        await this.$store.dispatch("power/toggleBlocs", true);
      }
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

      if (selectedBloc == -1) {
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

    async selectBloc(n) {
      //this.selectedBloc = n
      await this.$store.dispatch("power/setSelectedBloc", n);
    },
  },
};
</script>

<style>
p {
  margin: 0;
  padding: 0 0 0.5rem 1rem;
}

h4 {
  margin: 0.3rem 0;
}

h6 {
  margin: 0.3rem 0;
}

.inline {
  display: inline;
}

.flexbox {
  display: flex;
  justify-content: space-between;
}

.faicon {
  cursor: pointer;
  color: red;
  vertical-align: sub;
}

#velse {
  max-height: calc(100vh - 3.5rem);
  padding: 0.5rem 1rem;
  overflow: auto;
}

.blocSelectionButton {
  height: 30px;
  width: 30px;
  border-radius: 15px;
  border-color: blue;
  color: white;
  background-color: blue;
  margin: 0.75rem 0;
  align-content: center;
  display: inline;
  vertical-align: middle;
  padding: 0rem 0 1rem 0;
}
</style>