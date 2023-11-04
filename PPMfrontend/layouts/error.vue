<template>
    <div class="not-found">
        <img src="electricity.png" width="60rem" />
        <h1>404 - Not Found</h1>
        <p>A keresett oldal sajnos nem található.</p>
        <nuxt-link to="/">Vissza a kezdőoldalra</nuxt-link>
    </div>
</template>

<script>
export default {
    name: "NotFound",
    layout: "customErrorLayout",
    data() {
        return {
            statusCode: 404,
        };
    },
};
</script>

<style scoped>
.not-found {
    text-align: center;
    padding-top: 25vh;
}

h1 {
    padding-top: 1rem;
    font-size: 3rem;
}

p {
    font-size: 1.5rem;
    margin-top: 20px;
}

router-link {
    font-size: 1.2rem;
    margin-top: 20px;
    display: inline-block;
    text-decoration: underline;
}
</style>
