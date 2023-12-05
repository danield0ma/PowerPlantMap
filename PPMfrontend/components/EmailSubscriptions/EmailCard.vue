<template>
    <div class="card cardHover">
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'envelope']"
                :size="'lg'"
                class="faicon"
            />
            <h5 class="m-0">{{ subscription.email }}</h5>
        </div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'clock']"
                :size="'lg'"
                class="faicon green"
            />
            <p>{{ creationTime }}</p>
        </div>
        <font-awesome-icon
            :icon="['fas', 'trash']"
            :size="'lg'"
            class="faicon red"
            v-on:click="deleteSubscription"
        />
    </div>
</template>

<script>
import moment from "moment";

export default {
    name: "EmailCard",
    props: {
        subscription: Object,
    },

    computed: {
        creationTime() {
            return moment(this.subscription.created).format(
                "YYYY.MM.DD. HH:mm:SS"
            );
        },
    },

    methods: {
        async deleteSubscription() {
            if (!confirm("Biztosan törölni szeretnéd?")) return;
            await this.$axios.$get(
                "/api/EmailSubscriptions/Delete?email=" +
                    this.subscription.email
            );
            this.$emit("delete");
        },
    },
};
</script>

<style scoped>
p {
    margin: 0;
    vertical-align: middle;
}
</style>
