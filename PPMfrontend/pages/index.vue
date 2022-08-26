<template>
  <!-- <Tutorial/> -->
  <div>
    <div id="left" v-if="showLeftPanel">
      <div style="padding: 1rem; display: flex; justify-content: space-between; vertical-align: middle;">
        <icon v-on:click="close" style="cursor: pointer; color: red; width: 10rem; align-items: center; margin: auto;">
          <font-awesome-icon icon="fa-solid fa-arrow-left fa-2xl" />
          <font-awesome-icon icon="fa-solid fa-xmark fa-xs" />
        </icon>
        <h1>{{ content }}</h1>
      </div>
    </div>
    <div id="map" style="width: 100vw; height: 100vh;"></div>
  </div>  
</template>

<script>
import mapboxgl from "mapbox-gl"

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
  mounted() {
    this.createMap()
  },
  computed: {
    showLeftPanel() {
      return this.$store.state.power.left
    },
    content() {
      return this.$store.state.power.content
    }
  },
  methods: {
    createMap() {
      mapboxgl.accessToken = this.accessToken
      this.map = new mapboxgl.Map({
        accessToken: this.accessToken,
        container: 'map',
        style: 'mapbox://styles/danieldoma/cl6gnh6eg008l14pdjazw50fy',
        //style: 'mapbox://styles/danieldoma/cl7adhm06003314nzartd14sk',
        center: [19.4, 47],
        zoom: 6.75,
        maxZoom: 9,
        minZoom: 5
      })

      this.map.on('load', () => {

        this.map.loadImage(
          'atoms.png',
          //'https://docs.mapbox.com/mapbox-gl-js/assets/cat.png',
          (error, image) => {
          if (error) throw error;

        this.map.addSource('plants', {
            'type': 'geojson',
            'data': {
                'type': 'FeatureCollection',
                'features': [
              
              {
                'type': 'Feature',
                'properties': {
                  'name': 'Paks',
                  'description': '<p>Paksi Atomerőmű</p>'
                },
                'geometry': {
                  'type': 'Point',
                  'coordinates': [18.8526, 46.5753]
                }
                // 'name': 'Paks',
                // 'img': 'nuclear.png'
              }
              // {
              //   name: 'Mátra',
              //   lnglat: [20.0679, 47.7889],
              //   img: 'coal.png'
              // },
              // {
              //   name: 'Gönyű',
              //   lnglat: [17.8038, 47.7383],
              //   img: 'gas.png'
              // }
            ]
            }
        });
      
        this.map.addImage('cat', image)

        this.map.addLayer({
          'id': 'places',
          'type': 'symbol',
          'source': 'plants',
          // 'paint': {
          //   'circle-color': '#4264fb',
          //   'circle-radius': 15,
          //   'circle-stroke-width': 2,
          //   'circle-stroke-color': '#ffffff'
          // }
          'layout': {
            'icon-image': 'cat',
            'icon-size': 0.08
          }
        })

        const popup = new mapboxgl.Popup({
          closeButton: false,
          closeOnClick: false
        })

        this.map.on('mouseenter', 'places', (e) => {
            // Change the cursor style as a UI indicator.
            this.map.getCanvas().style.cursor = 'pointer';

            // Copy coordinates array.
            const coordinates = e.features[0].geometry.coordinates.slice();
            const description = e.features[0].properties.description;

            // Ensure that if the map is zoomed out such that multiple
            // copies of the feature are visible, the popup appears
            // over the copy being pointed to.
            while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
                coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
            }

            // Populate the popup and set its coordinates
            // based on the feature found.
            popup.setLngLat(coordinates).setHTML(description).addTo(this.map);
        });

        this.map.on('mouseleave', 'places', () => {
            this.map.getCanvas().style.cursor = '';
            popup.remove();
        });
      })
      })

      const coord = [
        {
          name: 'Paks',
          lnglat: [18.8526, 46.5753],
          img: 'nuclear.png',
          content: 'Paks'
        },
        {
          name: 'Mátra',
          lnglat: [20.0679, 47.7889],
          img: 'coal.png'
        },
        {
          name: 'Gönyű',
          lnglat: [17.8038, 47.7383],
          img: 'gas.png'
        }
      ]

      for (const marker of coord)
      {
        const element = document.createElement('div')
        element.className = 'marker'
        element.style.backgroundImage = `url("${marker.img}")`
        //element.style.backgroundImage = `url(https://placekitten.com/g/50/50/)`;
        element.style.width = `3rem`
        element.style.height = `3rem`
        element.style.backgroundSize = '100%'

        const m = new mapboxgl.Marker(element)
          .setLngLat(marker.lnglat)
          //.setPopup(new mapboxgl.Popup().setHTML('<h1>Paks</h1><h3>2000MW</h3>'))
          .addTo(this.map)

        m.getElement().addEventListener('click', () => {
          if (this.showLeftPanel && this.content == marker.name) {
            this.$store.dispatch('power/setLeftPanel', false)
          } else {
            this.$store.dispatch('power/setLeftPanel', true)
            this.$store.dispatch('power/setLeftContent', marker.name)
          }
        })
      }
      
    },
    close() {
      this.$store.dispatch('power/setLeftPanel', false)
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
    position: absolute;
    z-index: 1;
    background: rgba(255, 255, 255, 0.75);
    height: 100vh;
    width: 25vw;
    padding-top: 3.5rem;
  }

  .marker {
    display: block;
    border: none;
    /* border-radius: 50%; */
    cursor: pointer;
    padding: 0;
  } 
</style>
