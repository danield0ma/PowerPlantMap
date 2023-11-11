<template>
    <div class="container outer">
        <div class="row justify-content-center">
            <div class="col-md-5">
                <div style="text-align: center">
                    <img src="/electricity.png" width="60rem" />
                    <h1>Bejelentkezés</h1>
                </div>
                <form onsubmit="login">
                    <div class="form-group">
                        <label for="email">Felhasználónév:</label>
                        <input
                            type="text"
                            class="form-control"
                            id="email"
                            v-model="email"
                            required
                        />
                    </div>
                    <div class="form-group">
                        <label for="password">Jelszó:</label>
                        <input
                            type="password"
                            class="form-control"
                            id="password"
                            v-model="password"
                            required
                        />
                    </div>
                    <button type="submit" class="btn btn-primary">Login</button>
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
            email: "",
            password: "",
        };
    },
    head() {
        return {
            title: "Bejelentkezés",
        };
    },
    methods: {
        login() {},
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
