<template>
    <div class="outerDiv">
        <div id="innerDiv">
            <nuxt-link to="/admin">
                <img src="/electricity.png" width="30px" />
            </nuxt-link>
            <h4 style="margin: 0 0 0.1rem 0.75rem">
                <nuxt-link to="/admin">PowerPlantMap Admin</nuxt-link>
            </h4>
        </div>
        <ul>
            <p><nuxt-link to="/admin/powerplantseditor">Erőművek</nuxt-link></p>
            <p><nuxt-link to="/admin/email">E-mail lista</nuxt-link></p>
            <p @click="logout">Kijelentkezés</p>
        </ul>
    </div>
</template>

<script>
export default {
    name: "AdminHeader",

    methods: {
        async logout() {
            await this.$auth.logout();
            this.$router.push("/");
        },
    },
};
</script>

<style>
.outerDiv {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 0;
    padding: 0 2rem;
    /* background-color: gray; */
}

#innerDiv {
    display: flex;
}

ul {
    display: flex;
    margin: 0;
    text-decoration: none;
    text-transform: none;
}

ul p {
    padding: 1rem;
    text-decoration: none;
    margin: 0;
    color: blueviolet;
    cursor: pointer;
}

a:hover {
    text-decoration: none;
    color: darkgreen;
}
</style>
