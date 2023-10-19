<template>
  <div>
    <div style="height: 3.5rem; position: absolute"></div>
    <div id="left" v-if="showLeftPanel">
      <LeftPanel></LeftPanel>
    </div>
    <div id="rightPanel" v-if="rightNotLoading">
      <RightPanel :powerArray="powerOfPowerPlants" />
    </div>
    <div id="chooseDay">
      <p>Napválasztó</p>
      <input type="date" v-model="chosenDate" />
      <button
        v-on:click="setDate"
        class="btn btn-primary"
        style="margin-left: 0.5rem"
      >
        OK
      </button>
    </div>
    <div id="map"></div>
  </div>
</template>

<script>
import mapboxgl from "mapbox-gl";
import LeftPanel from "../components/LeftPanel.vue";
import RightPanel from "../components/RightPanel.vue";
import moment from "moment";

export default {

  name: "MapView",
  data() {
    return {
      accessToken: process.env.ACCESS_TOKEN,
      map: {},
      gj: {},
      powerOfPowerPlants: {},
      marker: [],
      popup: {},
      chosenDate: "",
      BASE_PATH: "https://powerplantmap.tech:5001/",
    };
  },

  head() {
    return {
      title: "Map View - PowerPlantMap",
    };
  },

  components: {
    LeftPanel,
    RightPanel,
  },

  mounted() {
    this.createMap();
    this.getLoad();
    this.defaultTime;
  },

  async asyncData() {
    const BASE_PATH = "https://powerplantmap.tech:5001/";
    const basics = await fetch(
      `${BASE_PATH}` + "API/Power/getPowerPlantBasics"
    );
    const features = await basics.json();
    const gj = {
      type: "geojson",
      data: {
        type: "FeatureCollection",
        features: features,
      },
    };

    const powers = await fetch(
      `${BASE_PATH}` + "API/Power/getPowerOfPowerPlants"
    );
    const powerOfPowerPlants = await powers.json();

    return { gj, powerOfPowerPlants };
  },

  computed: {
    showLeftPanel() {
      return this.$store.state.power.left;
    },

    rightNotLoading() {
      return !this.$store.state.power.rightLoading;
    },

    content() {
      return this.$store.state.power.content;
    },

    defaultTime() {
      let time = moment(Date(Date.now())).format("YYYY-MM-DD");
      this.chosenDate = time;
      //this.$store.dispatch('power/setDate', time)
      return time;
    },

    getDate() {
      return this.$store.state.power.date;
    },
  },

  methods: {
    async fetchWithBasePath(path) {
      const basePath = "https://powerplantmap.tech:5001/";
      const url = `${basePath}${path}`;
      console.log(url);
      return await fetch(url, {
        mode: "no-cors",
        contentType: "application/json",
        accessControlAllowOrigin: "*",
      });
    },

    async getLoad() {
      if (this.getDate != null) {
        const powerOfPowerPlantsResponse = await fetch(
          this.BASE_PATH +
            "API/Power/getPowerOfPowerPlants?date=" +
            this.getDate
        );
        this.powerOfPowerPlants = await powerOfPowerPlantsResponse.json();
      }
      await this.$store.dispatch("power/setRightLoading", false);
    },

    async createMap() {
      console.log(process.env);
      this.map = new mapboxgl.Map({
        accessToken: "pk.eyJ1IjoiZGFuaWVsZG9tYSIsImEiOiJjbDJvdDI1Mm4xNWZoM2NydWdxbWdvd3ViIn0.5x6xp0dGOMB_eh6_r_V79Q",
        container: "map",
        style: "mapbox://styles/danieldoma/cl6gnh6eg008l14pdjazw50fy",
        center: [19.7, 47.15],
        zoom: 6.75,
        maxZoom: 9,
        minZoom: 5,
      });

      const coord = this.gj.data.features;
      for (const marker of coord) {
        const element = document.createElement("div");
        element.className = "marker";
        element.style.backgroundImage = `url('${marker.properties.img}')`;
        element.style.width = `3rem`;
        element.style.height = `3rem`;
        element.style.backgroundSize = "100%";

        const m = new mapboxgl.Marker(element)
          .setLngLat(marker.geometry.coordinates)
          .addTo(this.map);

        m.getElement().addEventListener("click", () => {
          if (
            this.showLeftPanel &&
            this.content.powerPlantID == marker.properties.id
          ) {
            this.$store.dispatch("power/setLeftPanel", false);
            this.$store.dispatch("power/setSelectedBloc", -1);
            this.$store.dispatch("power/toggleBlocs", false);
          } else {
            this.getDetailsOfPowerPlant(marker.properties.id);
          }
        });
      }
    },

    async getPowerPlantBasics() {
      const res = await fetch(this.BASE_PATH + "API/Power/getPowerPlantBasics");
      const f = await res.json();

      const data = {
        type: "geojson",
        data: {
          type: "FeatureCollection",
          features: f,
        },
      };
      return data;
    },

    async getDetailsOfPowerPlant(id) {
      try {
        await this.$store.dispatch("power/setLeftPanelLoading", true);
        await this.$store.dispatch("power/toggleBlocs", false);
        await this.$store.dispatch("power/setSelectedBloc", -1);
        await this.$store.dispatch("power/setLeftPanel", true);

        let res;
        if (this.getDate == null) {
          res = await fetch(
            this.BASE_PATH + "API/Power/getDetailsOfPowerPlant?id=" + id
          );
        } else {
          res = await fetch(
            this.BASE_PATH +
              "API/Power/getDetailsOfPowerPlant?id=" +
              id +
              "&date=" +
              this.getDate
          );
        }
        const data = await res.json();

        await this.$store.dispatch("power/setLeftContent", data);
        await this.$store.dispatch("power/setLeftPanelLoading", false);
      } catch (error) {
        console.error(error);
      }
    },

    async setDate() {
      this.$store.dispatch("power/setRightLoading", true);
      this.$store.dispatch("power/setLeftPanel", false);
      await this.$store.dispatch("power/setDate", this.chosenDate);
      await this.getLoad();
    },
  },
};
</script>

<style>
body {
  margin: 0;
  padding: 0;
}

#left {
  display: block;
  position: absolute;
  z-index: 1;
  background: rgba(255, 255, 255, 0.75);
  height: auto;
  width: 33vw;
  margin-top: 3.5rem;
  bottom: 0;
  top: 0;
  /* left:0;
    right:0; */
}

#map {
  width: 100vw;
  height: 100vh;
  position: relative;
}

.marker {
  display: block;
  border: none;
  /* border-radius: 50%; */
  cursor: pointer;
  padding: 0;
}

#rightPanel {
  /* float: right; */
  display: block;
  position: absolute;
  z-index: 1;
  background: rgba(255, 255, 255, 0.75);
  height: calc(100vh - 3.5rem);
  /* height: auto; */
  width: 33vw;
  margin-top: 3.5rem;
  right: 0;
}

#innerRight {
  padding: 0.5rem 1rem;
}

.flexbox {
  display: flex;
  justify-content: space-between;
}

#chooseDay {
  margin: auto;
  /* display: block; */
  position: absolute;
  z-index: 1;
  background: rgba(255, 255, 255, 0.75);
  /* height: calc(100vh - 3.5rem); */
  width: 20vw;
  height: 80px;
  text-align: center;
  bottom: 2rem;
  left: 40vw;
  border-radius: 25px;
}
</style>
