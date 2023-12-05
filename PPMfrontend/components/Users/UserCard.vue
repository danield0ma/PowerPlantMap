<template>
    <div class="card cardHover">
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'user']"
                :size="'lg'"
                class="faicon blue"
            />
            <h4 class="m-0">{{ user.userName }}</h4>
        </div>
        <div class="d-flex justify-content-center align-items-center">
            <font-awesome-icon
                :icon="['fas', 'envelope']"
                :size="'lg'"
                class="faicon"
            />
            <h6 class="m-0">{{ user.email }}</h6>
        </div>
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
