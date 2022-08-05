<template>
  <div>
    <Header id="head" />
    <Nuxt id="body" /> 
  </div>
  
</template>

<script>
  import Header from '../components/Header'
  export default {
    components: {
      Header
    }
  }
</script>

<style>
  #head {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    /* background: rgba(236, 225, 204, 0.7); */
    /* background: rgba(245, 240, 240, 0.8); */
    background: rgba(255, 255, 255, 0.75);
    margin: 0;
    padding: 0;
    border: 0;  
    z-index: 2;  
    height: 3.5rem;
  }

  #body {
    position: relative;
    size: cover;
    height: 100vh;
    width: 100vw;
  }
</style>
