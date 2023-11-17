<template>
    <div>
        <b-navbar toggleable="lg" type="light" variant="light">
            <b-navbar-brand href="/">
                <img src="/electricity.png" width="30px" />
                PowerPlantMap
            </b-navbar-brand>
            <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav class="ml-auto">
                    <b-nav-item href="/">Map</b-nav-item>
                    <b-nav-item href="/statistics">Statistics</b-nav-item>
                    <b-nav-item href="/about">About</b-nav-item>
                    <b-nav-item href="/admin">Admin</b-nav-item>
                    <b-nav-item href="/register">Register</b-nav-item>
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>
    </div>
</template>

<script>
export default {
    name: "PpmHeader",

    mounted() {
        // Matomo
        var _paq = (window._paq = window._paq || []);
        _paq.push(["trackPageView"]);
        _paq.push(["enableLinkTracking"]);
        (function () {
            var u = "https://hallgato0.matomo.cloud/";
            _paq.push(["setTrackerUrl", u + "matomo.php"]);
            _paq.push(["setSiteId", "1"]);
            var d = document,
                g = d.createElement("script"),
                s = d.getElementsByTagName("script")[0];
            g.async = true;
            g.src = "//cdn.matomo.cloud/hallgato0.matomo.cloud/matomo.js";
            s.parentNode.insertBefore(g, s);
        })();
    },
};
</script>

<style scoped>
.navbar {
    height: 3.5rem !important;
    background-color: rgba(255, 255, 255, 0.75) !important;
    width: 100% !important;
}

.navbar-brand {
    font-size: 24px !important;
    padding-left: 1rem !important;
    color: blue !important;
}

.navbar-brand:hover {
    color: darkblue !important;
}

.nav-link {
    color: blue !important;
    padding: 0 1rem !important;
}

.nav-link:hover {
    color: darkblue !important;
}

@media (max-width: 768px) {
    .nav-link {
        margin: 0 !important;
        padding: 1rem 0 1rem 2rem !important;
        color: #333 !important;
        font-size: 18px !important;
        background-color: white !important;
        z-index: 2500 !important;
        color: blue !important;
    }
}
</style>
