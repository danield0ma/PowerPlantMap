<template>
    <div>
      <div style="height: 3.5rem; position: absolute;"></div>
      <div id="left" v-show="showLeftPanel">
        <div v-if="isLoading">
          <h1>LOADING...</h1>
        </div>
        <div v-else>
          <div style="padding: 1rem; display: flex; justify-content: space-between; vertical-align: middle;">
            <div>
              <h3>{{ content.description }}</h3>            
            </div>
            <font-awesome-icon icon="fa-solid fa-xmark fa-xs" v-on:click="close" style="cursor: pointer; color: red; vertical-align: baseline;" />
          </div>
          <h5>{{ content.operatorCompany }}</h5>
          <h5>{{ content.webpage }}</h5>
          <!-- <a href={{ content.webpage }}>{{ content.webpage }}</a> -->
          <h5>Maximális teljesítmény: {{ content.maxPower }} MW</h5>
          <h1>Blokkok</h1>
          <div v-for="bloc in content.blocs" :key="bloc.blocID">
            <p>{{bloc.blocID}}({{bloc.blocType}}): {{bloc.currentPower}} / {{bloc.maxBlocCapacity}}MW</p>
            <div v-for="generator in bloc.generators" :key="generator.generatorID">
              <p>     {{generator.generatorID}}: {{generator.currentPower}} / {{generator.maxCapacity}}MW</p>
            </div>
          </div>
          <!-- <h5>1. Blokk</h5>
          <h5>Típus: </h5> -->
        </div>        
      </div>
      <div id="map"></div>
    </div>  
  </template>
  
  <script>
  import mapboxgl from "mapbox-gl"
import { runInThisContext } from "vm"
  
  export default {
    name: 'IndexPage',
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

    mounted() {
      this.createMap()
    },

    computed: {
      showLeftPanel() {
        return this.$store.state.power.left
      },
      content() {
        return this.$store.state.power.content
      },
      isLoading() {
        return this.$store.state.power.isLoading
      }
    },

    methods: {
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
        console.log('coord: ', coord)

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
            } else {
              this.getDetailsOfPowerPlant(marker.properties.id)
              //this.$store.dispatch('power/setLeftContent', this.getDetailsOfPowerPlant(marker.properties.id))
            }
          })
        }
        
      },
  
      close() {
        this.$store.dispatch('power/setLeftPanel', false)
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
        console.log(data)
        return data
      },
  
      async getDetailsOfPowerPlant(id) {
        console.log('ID: ', id)
        try {
          //loading page
          await this.$store.dispatch('power/setLeftPanelLoading', true)
          await this.$store.dispatch('power/setLeftPanel', true)

          const res = await fetch('https://localhost:7032/API/Power/getDetailsOfPowerPlant?id=' + id)
          const data = await res.json()
          console.log(data)
          //return data
          await this.$store.dispatch('power/setLeftContent', data)
          await this.$store.dispatch('power/setLeftPanelLoading', false)
        } catch(error) {
          console.error(error)
        }
        
        // if (id == 'PKS') {
        //   return {
        //     'id': 'PKS',
        //     'name': 'Paks',
        //     'description': 'Paksi Atomerőmű',
        //     'operator': 'MVM Paksi atomeromu Zrt.',
        //     'webpage': 'https://www.atomeromu.hu',
        //     'maxPower': 2000,
        //     'blocks': [
        //       {
        //         'name': 'Paks 1',
        //         'birth': '1982',
        //         'current': 0,
        //         'maxPower': 506,
        //         'type': 'VVER-440',
        //         'turbines': [
        //           {
        //             'name': 'Paks_Gép_1',
        //             'maxPower': 250
        //           },
        //           {
        //             'name': 'Paks_Gép_2',
        //             'maxPower': 250
        //           }
        //         ]
        //       },
        //       {
        //         'name': 'Paks 2',
        //         'maxPower': 506,
        //         'current': 506,
        //         'type': 'VVER-440',
        //         'turbines': [
        //           {
        //             'name': 'Paks_Gép_3',
        //             'maxPower': 250
        //           },
        //           {
        //             'name': 'Paks_Gép_4',
        //             'maxPower': 250
        //           }
        //         ]
        //       },
        //       {
        //         'name': 'Paks 3',
        //         'maxPower': 506,
        //         'current': 506,
        //         'type': 'VVER-440',
        //         'turbines': [
        //           {
        //             'name': 'Paks_Gép_5',
        //             'maxPower': 250
        //           },
        //           {
        //             'name': 'Paks_Gép_6',
        //             'maxPower': 250
        //           }
        //         ]
        //       },
        //       {
        //         'name': 'Paks 4',
        //         'maxPower': 506,
        //         'current': 506,
        //         'type': 'VVER-440',
        //         'turbines': [
        //           {
        //             'name': 'Paks_Gép_7',
        //             'maxPower': 250
        //           },
        //           {
        //             'name': 'Paks_Gép_8',
        //             'maxPower': 250
        //           }
        //         ]
        //       }
        //     ]
        //   }
        // } else if (id == 'MTR') {
        //   return {
        //     'id': 'MTR',
        //     'description': 'Mátrai Szén',
        //     'operator': 'MVM',
        //     'maxPower': 1000,
        //     'blocks': [
        //       {
        //         'name': 'Mátra 1',
        //         'birth': '1962',
        //         'maxPower': 300,
        //         'current': 150,
        //         'type': 'Coal',
        //         'turbines': [
        //           {
        //             'name': 'Mátra_Gép_1',
        //             'maxPower': 150
        //           },
        //           {
        //             'name': 'Mátra_Gép_2',
        //             'maxPower': 150
        //           }
        //         ]
        //       },
        //       {
        //         'name': 'Mátra 2',
        //         'maxPower': 300,
        //         'current': 150,
        //         'type': 'Coal',
        //         'turbines': [
        //           {
        //             'name': 'Mátra_Gép_3',
        //             'maxPower': 150
        //           },
        //           {
        //             'name': 'Mátra_Gép_4',
        //             'maxPower': 150
        //           }
        //         ]
        //       }
        //     ]
        //   }
        // } else if (id == 'GNY') {
        //   return {
        //     'id': 'GNY',
        //     'description': 'Gönyűi gáz',
        //     'operator': 'MVM',
        //     'maxPower': 500,
        //     'blocks': [
        //       {
        //         'name': 'Gönyű 1',
        //         'birth': '2010',
        //         'maxPower': 500,
        //         'current': 300,
        //         'type': 'Gas',
        //         'turbines': [
        //           {
        //             'name': 'Gönyű_Gép_1',
        //             'maxPower': 250
        //           },
        //           {
        //             'name': 'Gönyű_Gép_2',
        //             'maxPower': 250
        //           }
        //         ]
        //       },
        //     ]
        //   }
        // }
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
  </style>
  