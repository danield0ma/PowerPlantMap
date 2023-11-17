<template>
    <div>
        <PpmHeader id="head" />
        <Nuxt id="body" />
    </div>
</template>

<script>
import PpmHeader from "../components/Headers/PpmHeader";
export default {
    name: "Default",

    components: {
        PpmHeader,
    },
};
</script>

<style scoped>
#head {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    /* background: rgba(255, 255, 255, 0.75); */
    border: 0;
    z-index: 2;
    height: 3.5rem;
}

@media (max-width: 768px) {
    #head {
        background: rgba(255, 255, 255);
    }
}

#body {
    position: relative;
    size: cover;
    height: 100vh;
    width: 100vw;
}
</style>
