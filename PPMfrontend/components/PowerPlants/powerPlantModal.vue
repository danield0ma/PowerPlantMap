<template>
    <div>
        <div class="modal-backdrop" v-on:click="$emit('close')"></div>
        <div class="shade-content"></div>
        <div class="modal-content" v-on:click.stop>
            <div class="modal-body">
                <h1 v-if="'name' in this.powerPlant">
                    Erőmű adatainak szerkesztése
                </h1>
                <h1 v-else>Új erőmű létrehozása</h1>
                <div class="text-left d-flex justify-content-center row g-2">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="id">Erőmű azonosító:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="id"
                                v-model="powerPlant.powerPlantId"
                                required
                                disabled
                            />
                        </div>
                        <div class="form-floating mb-3">
                            <input
                                type="text"
                                class="form-control"
                                id="name"
                                v-model="powerPlant.name"
                                required
                            />
                            <label for="name">Erőmű neve:</label>
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
                        <div class="form-group">
                            <label for="longitude">Longitude:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="longitude"
                                v-model="powerPlant.longitude"
                                required
                            />
                        </div>
                        <div class="form-group">
                            <label for="latitude">Latitude:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="latitude"
                                v-model="powerPlant.latitude"
                                required
                            />
                        </div>
                        <div class="form-group">
                            <label for="color">Szín:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="color"
                                v-model="powerPlant.color"
                                required
                            />
                        </div>
                        <div class="form-group">
                            <label for="address">Cím:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="address"
                                v-model="powerPlant.address"
                                required
                            />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="color">Szín:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="color"
                                v-model="powerPlant.color"
                                required
                            />
                        </div>
                        <div class="form-group">
                            <label for="address">Cím:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="address"
                                v-model="powerPlant.address"
                                required
                            />
                        </div>
                    </div>
                </div>
                <div>
                    <button
                        type="submit"
                        class="btn btn-success"
                        v-on:click="savePowerPlant"
                    >
                        Mentés
                    </button>
                    <button
                        v-on:click="$emit('close')"
                        class="btn btn-outline-secondary"
                    >
                        Bezárás
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: "PowerPlantModal",
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
    background-color: #f5f5f5;
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
    max-height: 90%;
    z-index: 1000;
    text-align: center;
}

.modal-body {
    max-height: calc(90vh - 40px); /* 90vh minus the total vertical padding */
    overflow: auto;
    padding: 20px;
}

.shade-content {
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
