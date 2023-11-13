<template>
    <div class="container outer">
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div style="text-align: center">
                    <img src="/electricity.png" width="60rem" />
                    <h1>Regisztráció</h1>
                </div>
                <form @submit.prevent="Register">
                    <div class="form-group">
                        <label for="username">Felhasználónév:</label>
                        <input
                            type="text"
                            class="form-control"
                            id="username"
                            v-model="registrationData.userName"
                            required
                        />
                    </div>
                    <div class="form-group">
                        <label for="email">E-mail cím:</label>
                        <input
                            type="text"
                            class="form-control"
                            id="email"
                            v-model="registrationData.email"
                            required
                        />
                    </div>
                    <div class="form-group">
                        <label for="password">Jelszó:</label>
                        <input
                            type="password"
                            class="form-control"
                            id="password"
                            v-model="registrationData.password"
                            required
                        />
                    </div>
                    <!-- <div class="form-group">
                        <label for="passwordAgain">Jelszó mégegyszer:</label>
                        <input
                            type="passwordAgain"
                            class="form-control"
                            id="password"
                            v-model="login.passwordAgain"
                            required
                        />
                    </div> -->
                    <button type="submit" class="btn btn-primary">
                        Regisztráció
                    </button>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "Register",

    layout: "empty",

    data() {
        return {
            registrationData: {
                userName: "",
                email: "",
                password: "",
            },
        };
    },

    head() {
        return {
            title: "Register - PowerPlantMap",
        };
    },

    methods: {
        async Register() {
            await this.$axios.$post("/api/Account/Register", this.registrationData, {
                headers: {
                    "Content-Type": "application/json",
                },
            });
            try {
                const response = await this.$auth.loginWith("local", {
                    data: this.registrationData,
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
}

h1 {
    padding-top: 1rem;
    padding-bottom: 1rem;
    font-size: 3rem;
}
</style>
