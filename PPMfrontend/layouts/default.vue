<template>
    <div>
        <!-- <div v-if="isMap"> -->
        <Header id="head" />
        <Nuxt id="body" />
        <!-- </div>
    <div v-else>
      <Header />
      <Nuxt />
    </div> -->
        <!-- <Header id="head" />
    <Nuxt id="body" v-show="isMap" />
    <Nuxt v-show="notMap" /> -->
    </div>
</template>

<script>
import Header from "../components/Header";
export default {
    components: {
        Header,
    },

    computed: {
        isMap() {
            return this.$route.path == "/map";
        },

        notMap() {
            return this.$route.path != "/map";
        },
    },
};
</script>

<style>
#head {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    background: rgba(255, 255, 255, 0.75);
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
