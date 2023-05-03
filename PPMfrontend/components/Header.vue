<template>
  <div class="outerDiv">
    <div id="innerDiv">
      <nuxt-link to="/"><img src="electricity.png" width="30px" /></nuxt-link>
      <h4 style="margin: 0 0 0.1rem 0.75rem">
        <nuxt-link to="/">PowerPlantMap</nuxt-link>
      </h4>
    </div>
    <ul>
      <!-- <p><nuxt-link to="/">Index</nuxt-link></p> -->
      <!-- <p><nuxt-link to="/svgmap">Svg</nuxt-link></p> -->
      <p><nuxt-link to="/map">Map</nuxt-link></p>
      <!-- <p><nuxt-link to="/list">List</nuxt-link></p> -->
      <p><nuxt-link to="/about">About</nuxt-link></p>
    </ul>
  </div>
</template>

<script>
export default {
  name: "Header",
  mounted() {
    var _paq = (window._paq = window._paq || []);
    /* tracker methods like "setCustomDimension" should be called before "trackPageView" */
    _paq.push(["trackPageView"]);
    _paq.push(["enableLinkTracking"]);
    (function () {
      var u = "https://tesztsthszehu.matomo.cloud/";
      _paq.push(["setTrackerUrl", u + "matomo.php"]);
      _paq.push(["setSiteId", "1"]);
      var d = document,
        g = d.createElement("script"),
        s = d.getElementsByTagName("script")[0];
      g.async = true;
      g.src = "//cdn.matomo.cloud/tesztsthszehu.matomo.cloud/matomo.js";
      s.parentNode.insertBefore(g, s);
    })();
  },
};
</script>

<style>
.outerDiv {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 0;
  padding: 0 2rem;
  /* background: rgba(255, 255, 255, 0.75); */
}

#innerDiv {
  display: flex;
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