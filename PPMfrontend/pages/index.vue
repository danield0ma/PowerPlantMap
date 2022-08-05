<template>
  <!-- <Tutorial/> -->
  <div>
    <div id="left">
      <div style="padding: 1rem;">
        <h2>PAKS</h2>
        <h4>1. Blokk - 500MW</h4>
        <h4>2. Blokk - 500MW</h4>
        <h4>3. Blokk - 500MW</h4>
        <h4>4. Blokk - 500MW</h4>
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
  methods: {
    createMap() {
      mapboxgl.accessToken = this.accessToken
      this.map = new mapboxgl.Map({
        accessToken: this.accessToken,
        container: 'map',
        style: 'mapbox://styles/danieldoma/cl6gnh6eg008l14pdjazw50fy',
        center: [19.4, 47],
        zoom: 6.75
      })

      this.marker.push(new mapboxgl.Marker({
        color: 'green',
        draggable: false
      })
        .setLngLat([18.8526, 46.5753])
        .setPopup(new mapboxgl.Popup().setHTML('<h1>Paks</h1><h3>2000MW</h3>'))
        .addTo(this.map))

      this.marker.push(new mapboxgl.Marker({
        color: 'black',
        draggable: false
      })
        .setLngLat([20.0679, 47.7889])
        .setPopup(new mapboxgl.Popup().setHTML('<h1>Mátra<h3>350MW</h3></h1>'))
        .addTo(this.map))

      this.marker.push(new mapboxgl.Marker({
        color: 'red',
        draggable: false
      })
        .setLngLat([17.8038, 47.7383])
        .setPopup(new mapboxgl.Popup().setHTML('<h1>Gönyű</h1><h3>400MW</h3>'))
        .addTo(this.map))

      // this.popup = new mapboxgl.Popup()
      //   .setLngLat([17.8038, 45.7383])
      //   .setHTML("<h1>Hello World!</h1>")
      //   .setMaxWidth("300px")
      //   .addTo(map);
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
</style>
