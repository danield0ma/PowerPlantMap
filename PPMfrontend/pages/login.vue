<template>
    <div class="container outer">
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div style="text-align: center">
                    <img src="/electricity.png" width="60rem" />
                    <h1>Bejelentkezés</h1>
                </div>
                <form @submit.prevent="userLogin">
                    <div class="form-group">
                        <label for="username">Felhasználónév:</label>
                        <input
                            type="text"
                            class="form-control"
                            id="username"
                            v-model="login.username"
                            required
                        />
                    </div>
                    <div class="form-group">
                        <label for="password">Jelszó:</label>
                        <input
                            type="password"
                            class="form-control"
                            id="password"
                            v-model="login.password"
                            required
                        />
                    </div>
                    <button type="submit" class="btn btn-primary">
                        Bejelentkezés
                    </button>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "Login",

    layout: "empty",

    data() {
        return {
            login: {
                username: "",
                password: "",
            },
        };
    },

    head() {
        return {
            title: "Login - PowerPlantMap",
        };
    },

    methods: {
        async userLogin() {
            try {
                const response = await this.$auth.loginWith("local", {
                    data: this.login,
                });
                this.$router.push("/admin");
            } catch (err) {
                console.log(err);
            }
        },
    },
};
</script>

<style scoped>
.container {
    padding-top: 20vh;
    max-width: 80%;
}

h1 {
    padding-top: 1rem;
    padding-bottom: 1rem;
    font-size: 3rem;
}
</style>
