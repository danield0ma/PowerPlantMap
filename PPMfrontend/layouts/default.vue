<template>
    <div>
        <PpmHeader id="head"/>
        <Nuxt id="body" />
    </div>
</template>

<script>
import PpmHeader from "../components/PpmHeader";
export default {
    name: "Default",

    components: {
        PpmHeader,
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
