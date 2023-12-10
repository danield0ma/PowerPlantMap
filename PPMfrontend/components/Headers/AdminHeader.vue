<template>
    <div>
        <b-navbar toggleable="lg" type="light" variant="light">
            <b-navbar-brand href="/admin">
                <img src="/electricity.png" width="30px" />
                PPM Admin
            </b-navbar-brand>
            <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav class="ml-auto">
                    <b-nav-item
                        href="/admin/powerplantseditor"
                        class="align-self-center"
                        v-if="['admin', 'user'].includes(this.$auth.user.role)"
                        >Erőművek</b-nav-item
                    >
                    <b-nav-item
                        href="/admin/email"
                        class="align-self-center"
                        v-if="['admin', 'user'].includes(this.$auth.user.role)"
                        >E-mail lista</b-nav-item
                    >
                    <b-nav-item
                        href="/admin/users"
                        class="align-self-center"
                        v-if="this.$auth.user.role == 'admin'"
                        >Felhasználók</b-nav-item
                    >
                    <div class="d-flex justify-content-center pl-4">
                        <font-awesome-icon
                            icon="fa-solid fa-user"
                            class="faicon green p-0"
                            :size="'lg'"
                        />
                        <b-nav-item
                            class="m-0 pl-0 align-self-center green welcome"
                        >
                            Üdv, {{ this.$auth.user.username }}!
                        </b-nav-item>
                    </div>
                    <b-nav-item @click="logout" class="align-self-center"
                        >Kijelentkezés</b-nav-item
                    >
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>
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

<style scoped>
.navbar {
    height: 3.5rem !important;
    background-color: rgba(255, 255, 255) !important;
    width: 100% !important;
}

.navbar-brand {
    font-size: 24px !important;
    padding-left: 1rem !important;
    color: green !important;
}

.navbar-brand:hover {
    color: darkgreen !important;
}

.nav-link {
    color: green !important;
    padding: 0 1rem;
}

.nav-link:hover {
    color: darkgreen !important;
}

@media (max-width: 768px) {
    .nav-link {
        margin: 0 !important;
        padding: 1rem 0 1rem 2rem;
        color: #333 !important;
        font-size: 18px !important;
        background-color: white !important;
        z-index: 2500 !important;
        color: green !important;
    }
}

.welcome {
    padding-left: 0 !important;
}
</style>
