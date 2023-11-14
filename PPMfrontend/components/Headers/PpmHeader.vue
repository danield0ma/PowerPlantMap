<template>
    <div class="outerDiv">
        <div id="innerDiv">
            <nuxt-link to="/">
                <img src="/electricity.png" width="30px" />
            </nuxt-link>
            <h4 style="margin: 0 0 0.1rem 0.75rem">
                <nuxt-link to="/">PowerPlantMap</nuxt-link>
            </h4>
        </div>
        <b-navbar toggleable="md" type="dark" variant="info">
            <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav class="ml-auto">
                    <b-nav-item href="/statistics">Statistics</b-nav-item>
                    <b-nav-item href="/about">About</b-nav-item>
                    <b-nav-item href="/login">Login</b-nav-item>
                    <b-nav-item href="/register">Register</b-nav-item>
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>
    </div>
</template>

<script>
export default {
    name: "PpmHeader",
};
</script>

<style>
.navbar {
    background-color: rgba(255, 255, 255, 0.75) !important;
    width: auto !important;
    position: absolute !important;
    right: 0 !important;
}

.navbar-toggler {
    background-color: gray;
}

.navbar-collapse {
    position: absolute !important;
    /* top: 3.5rem !important; */
    /* background-color: gray; */
    color: blue;
    /* padding: 1rem; */
}

.navbar-nav .nav-link {
    color: blue !important;
}

.navbar-nav .nav-link:hover {
    color: darkblue !important;
}

.outerDiv {
    width: 100vw !important;
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

a:hover {
    text-decoration: none;
    color: darkblue;
}
</style>
