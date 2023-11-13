<template>
    <div class="About">
        <h1>PowerPlantMap User lista</h1>
        <div>
            <h2>Felhasználók</h2>
            <div v-for="user in users" :key="user.Id">
                <p>{{ user.userName }}</p>
                <p>{{ user.email }}</p>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "Email",

    layout: "adminLayout",

    middleware: "authenticated",

    head() {
        return {
            title: "Email lista szerkesztő - PowerPlantMap",
        };
    },

    data() {
        return {
            users: [],
        };
    },

    async asyncData({ $auth, $axios }) {
        const users = await $axios.$get("/api/Account/Get", {
            headers: {
                "Content-Type": "application/json",
                Authorization: $auth.getToken("local"),
            },
        });
        return { users };
    },
};
</script>

<style>
body {
    margin: 0;
    padding: 0;
}

.About {
    background-color: #808080;
    overflow-y: auto;
    padding-top: 4rem;
    text-align: center;
    margin: auto;
}
</style>
