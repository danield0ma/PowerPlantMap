<template>
    <div class="content">
        <h1>Admin Index</h1>
        <div class="pt-6 d-flex row justify-content-around mt-5">
            <NuxtLink
                to="/admin/powerplantseditor"
                class="col-12 col-md-3 card cardHover adminCard text-decoration-none"
                ><h5>Erőművek szerkesztése</h5></NuxtLink
            >
            <NuxtLink
                to="/admin/email"
                class="col-12 col-md-3 card cardHover adminCard text-decoration-none"
            >
                <h5>E-mail lista szerkesztése</h5>
            </NuxtLink>
            <NuxtLink
                to="/admin/users"
                class="col-12 col-md-3 card cardHover adminCard text-decoration-none"
                ><h5>Felhasználók kezelése</h5></NuxtLink
            >
        </div>
    </div>
</template>

<script>
export default {
    name: "AdminIndex",
    layout: "adminLayout",
    middleware: "authenticated",
    head() {
        return {
            title: "Admin - PowerPlantMap",
        };
    },
};
</script>

<style scoped>
.adminCard {
    cursor: pointer;
    padding: 7rem;
    margin: 1rem;
}
</style>
