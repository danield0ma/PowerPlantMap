<template>
    <div>
        <div class="modal-backdrop" v-on:click="$emit('close')"></div>
        <div class="behind-content"></div>
        <div class="modal-content" v-on:click.stop>
            <h1 v-if="'name' in this.powerPlant">
                Erőmű adatainak szerkesztése
            </h1>
            <h1 v-else>Új erőmű létrehozása</h1>
            <form class="col-md-6 text-left">
                <div class="form-group">
                    <label for="id">Id:</label>
                    <input
                        type="text"
                        class="form-control"
                        id="id"
                        v-model="powerPlant.id"
                        required
                    />
                </div>
                <div class="form-group">
                    <label for="name">Név:</label>
                    <input
                        type="text"
                        class="form-control"
                        id="name"
                        v-model="powerPlant.name"
                        required
                    />
                </div>
                <div class="form-group">
                    <label for="description">Leírás:</label>
                    <input
                        type="text"
                        class="form-control"
                        id="description"
                        v-model="powerPlant.description"
                        required
                    />
                </div>
                <div class="form-group">
                    <label for="operator">Üzemeltető cég:</label>
                    <input
                        type="text"
                        class="form-control"
                        id="operator"
                        v-model="powerPlant.operatorCompany"
                        required
                    />
                </div>
                <div class="form-group">
                    <label for="webpage">Weboldal:</label>
                    <input
                        type="text"
                        class="form-control"
                        id="webpage"
                        v-model="powerPlant.webpage"
                        required
                    />
                </div>
                <!-- Longitude: "",
                Latitude: "",
                Color: "",
                Address: "", -->
                <button
                    type="submit"
                    class="btn btn-primary"
                    v-on:click="savePowerPlant"
                >
                    Mentés
                </button>
                <button v-on:click="$emit('close')" class="btn btn-warning">
                    Bezárás
                </button>
            </form>
        </div>
    </div>
</template>

<script>
export default {
    name: "AddPowerPlant",
    props: {
        powerPlant: Object,
    },

    // data() {
    //     return {
    //         Id: "",
    //         Name: "",
    //         Description: "",
    //         OperatorCompany: "",
    //         Webpage: "",
    //         Longitude: "",
    //         Latitude: "",
    //         Color: "",
    //         Address: "",
    //         IsCountry: "",
    //         Blocs: [],
    //     };
    // },

    methods: {
        savePowerPlant() {
            // console.log(
            //     this.Id,
            //     this.Name,
            //     this.Description,
            //     this.OperatorCompany,
            //     this.Webpage
            // );
            console.log(this.powerPlant);
        },
    },
};
</script>

<style>
.modal-backdrop {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgb(0, 0, 0);
    opacity: 0.75 !important;
    z-index: 998;
}

.modal-content {
    background-color: #fff;
    color: #000;
    padding: 20px;
    border-radius: 15px;
    border: 3px solid #333;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    opacity: 1;
    max-width: 90%;
    z-index: 1000;
    text-align: center;
}

.behind-content {
    background-color: #000;
    opacity: 1;
    border-radius: 15px;
    border: 3px solid #333;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    max-width: 90%;
    z-index: 999;
}
</style>
