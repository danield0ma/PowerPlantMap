<template>
  <div>
    <div style="height: 3.5rem; position: absolute;"></div>
    <div id="left" v-if="showLeftPanel">
      <LeftPanel></LeftPanel>
    </div>
    <div id="rightPanel" v-if="rightNotLoading">
      <RightPanel></RightPanel>
    </div>
    <div id="chooseDay">
      <p>Napválasztó</p>
    </div>
    <div id="map"></div>
  </div>
</template>
  
<script>
import mapboxgl from "mapbox-gl"
import LeftPanel from '../components/LeftPanel.vue'
import RightPanel from '../components/RightPanel.vue'

export default {
    name: 'MapView',
    data() {
      return {
        accessToken: 'pk.eyJ1IjoiZGFuaWVsZG9tYSIsImEiOiJjbDJvdDI1Mm4xNWZoM2NydWdxbWdvd3ViIn0.5x6xp0dGOMB_eh6_r_V79Q',
        map: {},
        marker: [],
        popup: {}
      }
    },

    head() {
      return {
        title: 'Map View - PowerPlantMap'
      }
    },

    components: {
      LeftPanel,
      RightPanel
    },

    mounted() {
      this.createMap()
      this.getLoad()
    },

    computed: {
      showLeftPanel() {
        return this.$store.state.power.left
      },

      rightNotLoading() {
        return !this.$store.state.power.rightLoading
      },

      content() {
        return this.$store.state.power.content
      }
    },

    methods: {
      async getLoad() {
        const currentLoadResponse = await fetch('https://localhost:7032/API/Power/getCurrentLoad/')
        const currentLoad = await currentLoadResponse.json()
        this.$store.dispatch('power/setCurrentLoad', currentLoad)

        const loadHistoryResponse = await fetch('https://localhost:7032/API/Power/getLoadHistory/')
        const loadHistory = await loadHistoryResponse.json()
        this.$store.dispatch('power/setLoadHistory', loadHistory)

        const powerOfPowerPlantsResponse = await fetch('https://localhost:7032/API/Power/getPowerOfPowerPlants/')
        const powerOfPowerPlants = await powerOfPowerPlantsResponse.json()
        this.$store.dispatch('power/setPowerOfPowerPlants', powerOfPowerPlants)

        this.$store.dispatch('power/setRightLoading', false)
      },

      async createMap() {
        mapboxgl.accessToken = this.accessToken
        this.map = new mapboxgl.Map({
          accessToken: this.accessToken,
          container: 'map',
          style: 'mapbox://styles/danieldoma/cl6gnh6eg008l14pdjazw50fy',
          center: [19.4, 47],
          zoom: 6.75,
          maxZoom: 9,
          minZoom: 5
        })
  
        // this.map.on('load', () => {
        //   this.map.loadImage('atoms.png',// 'coal.png',
        //     (error, image) => {
              
        //       if (error) throw error
  
        //       this.map.addSource('plants', this.geojson());
            
        //       this.map.addImage('cat', image)
        //       //this.map.addImage('coal', img)
  
        //       this.map.addLayer({
        //         'id': 'places',
        //         //'type': 'symbol',
        //         'type': 'fill',
        //         'source': 'plants',
        //         // 'paint': {
        //         //   'circle-color': '#4264fb',
        //         //   'circle-radius': 15,
        //         //   'circle-stroke-width': 2,
        //         //   'circle-stroke-color': '#ffffff'
        //         // }
        //         // 'layout': {
        //         //   'icon-image': 'cat',
        //         //   'icon-size': 5
        //         // }
        //         'paint': {
        //           'fill-color': rgba(100, 100, 100, 0.5),
        //           'fill-pattern': 'cat'
        //         }
        //       })
  
        //       const popup = new mapboxgl.Popup({
        //         closeButton: false,
        //         closeOnClick: false
        //       })
  
        //       this.map.on('mouseenter', 'places', (e) => {
        //           // Change the cursor style as a UI indicator.
        //           this.map.getCanvas().style.cursor = 'pointer';
  
        //           // Copy coordinates array.
        //           const coordinates = e.features[0].geometry.coordinates.slice();
        //           const description = e.features[0].properties.description;
  
        //           // Ensure that if the map is zoomed out such that multiple
        //           // copies of the feature are visible, the popup appears
        //           // over the copy being pointed to.
        //           while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
        //               coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
        //           }
  
        //           // Populate the popup and set its coordinates
        //           // based on the feature found.
        //           popup.setLngLat(coordinates).setHTML(description).addTo(this.map);
        //       });
  
        //       this.map.on('mouseleave', 'places', () => {
        //           this.map.getCanvas().style.cursor = '';
        //           popup.remove();
        //       });
        //     })
        // })
  
        const gj = await this.getPowerPlantBasics()
        const coord = gj.data.features
        //console.log('coord: ', coord)

        for (const marker of coord)
        {
          const element = document.createElement('div')
          element.className = 'marker'
          element.style.backgroundImage = `url("${marker.properties.img}")`
          //element.style.backgroundImage = `url(https://placekitten.com/g/50/50/)`;
          element.style.width = `3rem`
          element.style.height = `3rem`
          element.style.backgroundSize = '100%'
  
          const m = new mapboxgl.Marker(element)
            .setLngLat(marker.geometry.coordinates)
            //.setPopup(new mapboxgl.Popup().setHTML('<h1>Paks</h1><h3>2000MW</h3>'))
            .addTo(this.map)
  
          m.getElement().addEventListener('click', () => {
            if (this.showLeftPanel && this.content.powerPlantID == marker.properties.id) {
              this.$store.dispatch('power/setLeftPanel', false)
              this.$store.dispatch('power/toggleBlocs', false)
            } else {
              this.getDetailsOfPowerPlant(marker.properties.id)
            }
          })
        }  
      },

      async getPowerPlantBasics() {
        const res = await fetch('https://localhost:7032/API/Power/getPowerPlantBasics/')
        const f = await res.json()
        
        const data = {
              'type': 'geojson',
              'data': {
                'type': 'FeatureCollection',
                'features': f
              }
            }
        //console.log(data)
        return data
      },
  
      async getDetailsOfPowerPlant(id) {
        //console.log('ID: ', id)
        try {
          //loading page
          await this.$store.dispatch('power/setLeftPanelLoading', true)
          this.$store.dispatch('power/toggleBlocs', false)
          await this.$store.dispatch('power/setLeftPanel', true)

          const res = await fetch('https://localhost:7032/API/Power/getDetailsOfPowerPlant?id=' + id)
          const data = await res.json()
          //console.log(data)
          //return data
          
          await this.$store.dispatch('power/setLeftContent', data)
          await this.$store.dispatch('power/setLeftPanelLoading', false)
        } catch(error) {
          console.error(error)
        }
      }
    }
  }
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
    width: 25vw;
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
  