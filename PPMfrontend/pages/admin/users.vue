<template>
    <div class="content">
        <h1>PowerPlantMap Felhasználók</h1>
        <div v-for="user in users" :key="user.Id">
            <p>{{ user.userName }}</p>
            <p>{{ user.email }}</p>
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
