<template>
    <div class="content">
        <h1>PowerPlantMap E-mail lista</h1>
        <div
            v-for="emailSubscription in emailSubscriptions"
            :key="emailSubscription.Id"
            class="d-flex justify-content-center"
        >
            <EmailCard
                :subscription="emailSubscription"
                @delete="deleteSubscription(emailSubscription.id)"
            ></EmailCard>
        </div>
    </div>
</template>

<script>
import EmailCard from "../../components/EmailSubscriptions/EmailCard";

export default {
    name: "Email",

    layout: "adminLayout",

    middleware: "user",

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

    methods: {
        deleteSubscription(id) {
            this.emailSubscriptions = this.emailSubscriptions.filter(
                (subscription) => subscription.id !== id
            );
        },
    },
};
</script>
