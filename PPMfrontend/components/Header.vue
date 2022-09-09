<template>
    <div class="outerDiv">
      <div id="innerDiv">
        <nuxt-link to="/"><img src="electricity.png" width="30px"></nuxt-link>
        <h4><nuxt-link to="/">PowerPlantMap</nuxt-link></h4>
      </div>
      <ul>
        <p><nuxt-link to="/map">Map</nuxt-link></p>
        <p><nuxt-link to="/list">List</nuxt-link></p>
        <p><nuxt-link to="/about">About</nuxt-link></p>
      </ul>
    </div>    
</template>

<style>
  .outerDiv {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 0;
    padding: 0 2rem;
  }

  #innerDiv {
    display: flex;
  }

  h4 {
    margin: 0;
    color: orange;
    padding-left: 1rem;
    text-decoration: none;
  }

  ul {
    display: flex;
    margin: 0;
    text-decoration: none;
    text-transform: none;
  }

  ul p {
    padding: 1rem;
    text-decoration: none;
    margin: 0;
    color: blue;
  }

  nuxt-link {
    text-decoration: none;
  }
</style>