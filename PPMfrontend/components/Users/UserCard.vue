<template>
    <div class="card cardHover">
        <p>{{ user.userName }}</p>
        <p>{{ user.email }}</p>
    </div>
</template>

<script>
export default {
    name: "UserCard",
    props: {
        user: Object,
    },
};
</script>

<style scoped></style>
