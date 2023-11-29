<template>
    <div class="card cardHover">
        <p>{{ user.userName }}</p>
        <p>{{ user.email }}</p>
        <font-awesome-icon
            :icon="['fas', 'trash']"
            :size="'lg'"
            class="faicon red"
            v-on:click="deleteUser"
        />
    </div>
</template>

<script>
export default {
    name: "UserCard",
    props: {
        user: Object,
    },

    methods: {
        async deleteUser() {
            if (!confirm("Biztosan törölni szeretnéd?")) return;
            await this.$axios.$delete(
                "/api/Account/DeleteUser?userName=" + this.user.userName
            );
            this.$emit("delete");
        },
    },
};
</script>

<style scoped></style>
