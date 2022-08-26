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
        center: [19.4, 47],
        zoom: 6.75,
        maxZoom: 9,
        minZoom: 5
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
          .setPopup(new mapboxgl.Popup().setHTML('<h1>Paks</h1><h3>2000MW</h3>'))
          .addTo(this.map)

        m.getElement().addEventListener('click', () => {
          this.$store.dispatch('power/setLeftPanel', true)
          this.$store.dispatch('power/setLeftContent', marker.name)
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
