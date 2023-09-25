<template>
  <div>
    <div style='height: 3.5rem; position: absolute'></div>
    <div id='left' v-if='showLeftPanel'>
      <LeftPanel></LeftPanel>
    </div>
    <div id='rightPanel' v-if='rightNotLoading'>
      <RightPanel></RightPanel>
    </div>
    <div id='chooseDay'>
      <p>Napválasztó</p>
      <input type='date' v-model='chosenDate' />
      <button
        v-on:click='setDate'
        class='btn btn-primary'
        style='margin-left: 0.5rem'
      >
        OK
      </button>
    </div>
    <div id='map'></div>
  </div>
</template>
  
<script>
import mapboxgl from 'mapbox-gl';
import LeftPanel from '../components/LeftPanel.vue';
import RightPanel from '../components/RightPanel.vue';
import moment from 'moment';

export default {
  name: 'MapView',
  data() {
    return {
      accessToken:
        'pk.eyJ1IjoiZGFuaWVsZG9tYSIsImEiOiJjbDJvdDI1Mm4xNWZoM2NydWdxbWdvd3ViIn0.5x6xp0dGOMB_eh6_r_V79Q',
      map: {},
      marker: [],
      popup: {},
      chosenDate: '',
    };
  },

  head() {
    return {
      title: 'Map View - PowerPlantMap',
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
      let time = moment(Date(Date.now())).format('YYYY-MM-DD');
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
      const basePath = 'https://207.154.199.39:5001/';
      const url = `${basePath}${path}`;
      return await fetch(url);
    },

    async getLoad() {
      let powerOfPowerPlantsResponse;
      if (this.getDate == null) {
        powerOfPowerPlantsResponse = await this.fetchWithBasePath('API/Power/getPowerOfPowerPlants')
      } else {
        powerOfPowerPlantsResponse = await this.fetchWithBasePath('API/Power/getPowerOfPowerPlants?date=' + this.getDate)
      }
      const powerOfPowerPlants = await powerOfPowerPlantsResponse.json();
      await this.$store.dispatch(
        'power/setPowerOfPowerPlants',
        powerOfPowerPlants
      );
      await this.$store.dispatch('power/setRightLoading', false);
    },

    async createMap() {
      mapboxgl.accessToken = this.accessToken;
      this.map = new mapboxgl.Map({
        accessToken: this.accessToken,
        container: 'map',
        style: 'mapbox://styles/danieldoma/cl6gnh6eg008l14pdjazw50fy',
        center: [19.7, 47.15],
        zoom: 6.75,
        maxZoom: 9,
        minZoom: 5,
      });

      const gj = await this.getPowerPlantBasics();
      const coord = gj.data.features;
      //console.log('coord: ', coord)

      for (const marker of coord) {
        const element = document.createElement('div');
        element.className = 'marker';
        element.style.backgroundImage = `url('${marker.properties.img}')`;
        //element.style.backgroundImage = `url(https://placekitten.com/g/50/50/)`;
        element.style.width = `3rem`;
        element.style.height = `3rem`;
        element.style.backgroundSize = '100%';

        const m = new mapboxgl.Marker(element)
          .setLngLat(marker.geometry.coordinates)
          //.setPopup(new mapboxgl.Popup().setHTML('<h1>Paks</h1><h3>2000MW</h3>'))
          .addTo(this.map);

        m.getElement().addEventListener('click', () => {
          if (
            this.showLeftPanel &&
            this.content.powerPlantID == marker.properties.id
          ) {
            this.$store.dispatch('power/setLeftPanel', false);
            this.$store.dispatch('power/setSelectedBloc', -1);
            this.$store.dispatch('power/toggleBlocs', false);
          } else {
            this.getDetailsOfPowerPlant(marker.properties.id);
          }
        });
      }
    },

    async getPowerPlantBasics() {
      const res = await this.fetchWithBasePath('API/Power/getPowerPlantBasics');
      const f = await res.json();

      const data = {
        type: 'geojson',
        data: {
          type: 'FeatureCollection',
          features: f,
        },
      };
      //console.log(data)
      return data;
    },

    async getDetailsOfPowerPlant(id) {
      //console.log('ID: ', id)
      try {
        //loading page
        await this.$store.dispatch('power/setLeftPanelLoading', true);
        await this.$store.dispatch('power/toggleBlocs', false);
        await this.$store.dispatch('power/setSelectedBloc', -1);
        await this.$store.dispatch('power/setLeftPanel', true);

        let res;
        if (this.getDate == null) {
          res = await this.fetchWithBasePath('API/Power/getDetailsOfPowerPlant?id=' + id);
        } else {
          res = await this.fetchWithBasePath('API/Power/getDetailsOfPowerPlant?id=' + id + '&date=' + this.getDate);
        }
        const data = await res.json();

        await this.$store.dispatch('power/setLeftContent', data);
        await this.$store.dispatch('power/setLeftPanelLoading', false);
      } catch (error) {
        //console.error(error)
      }
    },

    async setDate() {
      this.$store.dispatch('power/setRightLoading', true);
      this.$store.dispatch('power/setLeftPanel', false);
      await this.$store.dispatch('power/setDate', this.chosenDate);
      //console.log('SETDATE: ', this.$store.state.power.date, this.chosenDate)
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
  