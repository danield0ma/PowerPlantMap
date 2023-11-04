<template>
    <div class="outerDiv">
        <div id="innerDiv">
            <nuxt-link to="/">
                <img src="electricity.png" width="30px" />
            </nuxt-link>
            <h4 style="margin: 0 0 0.1rem 0.75rem">
                <nuxt-link to="/">PowerPlantMap</nuxt-link>
            </h4>
        </div>
        <ul>
            <p><nuxt-link to="/">Map</nuxt-link></p>
            <p><nuxt-link to="/admin">Admin</nuxt-link></p>
            <p><nuxt-link to="/about">About</nuxt-link></p>
        </ul>
    </div>
</template>

<script>
export default {
    name: "PpmHeader",
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
