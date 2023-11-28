<template>
    <div class="content">
        <h1>PowerPlantMap Felhasználók</h1>
        <div
            v-for="user in users"
            :key="user.id"
            class="d-flex justify-content-center"
        >
            <UserCard :user="user"></UserCard>
        </div>
    </div>
</template>

<script>
import UserCard from "../../components/Users/UserCard";

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
    components: { UserCard },
};
</script>
