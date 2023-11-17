<template>
    <div class="About">
        <h1>PowerPlantMap Email lista</h1>
        <div>
            <h2>E-mail címek</h2>
            <div
                v-for="emailSubscription in emailSubscriptions"
                :key="emailSubscription.Id"
            >
                <p>{{ emailSubscription.created }}</p>
                <p>{{ emailSubscription.email }}</p>
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
            emailSubscriptions: [],
        };
    },

    async asyncData({ $auth, $axios }) {
        const emailSubscriptions = await $axios.$get(
            "/api/EmailSubscriptions/Get",
            {
                headers: {
                    "Content-Type": "application/json",
                    Authorization: $auth.getToken("local"),
                },
            }
        );
        return { emailSubscriptions };
    },
};
</script>

<style scoped>

.About {
    background-color: #808080;
    overflow-y: auto;
    padding-top: 4rem;
    text-align: center;
    margin: auto;
}
</style>
