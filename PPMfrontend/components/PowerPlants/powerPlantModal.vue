<template>
    <div>
        <div class="modal-backdrop" v-on:click="$emit('close')"></div>
        <div class="shade-content"></div>
        <div class="modal-content" v-on:click.stop>
            <div class="modal-body">
                <h1 v-if="isEditing">
                    {{ powerPlant.description }} adatainak szerkesztése
                </h1>
                <h1 v-else>Új erőmű létrehozása</h1>
                <div class="text-left d-flex justify-content-around">
                    <div class="col-md-5 card">
                        <h5 class="text-center">Erőmű adatai</h5>
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
                        <div class="form-group">
                            <label for="name">Erőmű neve:</label>
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
                            <label for="color">Szín (HTML):</label>
                            <input
                                type="text"
                                class="form-control"
                                id="color"
                                v-model="'#' + powerPlant.color"
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
                    <div class="col-md-5 card">
                        <h5 class="text-center">Erőmű blokkjai</h5>
                        <div v-if="isEditing">
                            <div
                                v-for="bloc in powerPlant.blocs"
                                :key="bloc.blocId"
                                class="card"
                            >
                                <div
                                    class="d-flex justify-content-center align-items-center"
                                >
                                    <h5 class="text-center">
                                        {{ bloc.blocId }}
                                    </h5>
                                    <font-awesome-icon
                                        :icon="['fas', 'trash']"
                                        class="faicon red"
                                    />
                                </div>
                                <div class="form-group">
                                    <label for="blocId">Blokk azonosító:</label>
                                    <input
                                        type="text"
                                        class="form-control"
                                        id="blocId"
                                        v-model="bloc.blocId"
                                        required
                                    />
                                </div>
                                <div class="form-group">
                                    <label for="blocType">Blokk típus:</label>
                                    <input
                                        type="text"
                                        class="form-control"
                                        id="blocType"
                                        v-model="bloc.blocType"
                                        required
                                    />
                                </div>
                                <div class="form-group">
                                    <label for="commissionDate"
                                        >Blokk üzemének kezdete (év):</label
                                    >
                                    <input
                                        type="text"
                                        class="form-control"
                                        id="commissionDate"
                                        v-model="bloc.commissionDate"
                                        required
                                    />
                                </div>
                                <div class="form-group">
                                    <label for="maxBlocCapacity"
                                        >Blokk teljesítménye [MW]:</label
                                    >
                                    <input
                                        type="text"
                                        class="form-control"
                                        id="maxBlocCapacity"
                                        v-model="bloc.maxBlocCapacity"
                                        required
                                    />
                                </div>
                                <h5 class="text-center">Blokk generátorai</h5>
                                <div
                                    v-for="generator in bloc.generators"
                                    :key="generator.generatorId"
                                    class="card"
                                >
                                    <h5 class="text-center">
                                        {{ generator.generatorId }}
                                    </h5>
                                    <div class="form-group">
                                        <label for="generatorId"
                                            >Generátor azonosítója:</label
                                        >
                                        <input
                                            type="text"
                                            class="form-control"
                                            id="generatorId"
                                            v-model="generator.generatorId"
                                            required
                                        />
                                    </div>
                                    <div class="form-group">
                                        <label for="maxCapacity"
                                            >Generátor maximális teljesítménye
                                            [MW]:</label
                                        >
                                        <input
                                            type="text"
                                            class="form-control"
                                            id="maxCapacity"
                                            v-model="generator.maxCapacity"
                                            required
                                        />
                                    </div>
                                    <button
                                        class="btn btn-success d-flex justify-content-center align-items-center p-0"
                                    >
                                        Generátor hozzáadása
                                        <font-awesome-icon
                                            :icon="['fas', 'plus']"
                                            class="faicon white"
                                        />
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div v-else class="pt-3"></div>
                        <button
                            class="btn btn-success d-flex justify-content-center align-items-center p-0 ml-2 mr-2"
                        >
                            Blokk hozzáadása
                            <font-awesome-icon
                                :icon="['fas', 'plus']"
                                class="faicon white"
                            />
                        </button>
                    </div>
                </div>
                <div>
                    <button
                        v-on:click="$emit('close')"
                        class="btn btn-outline-secondary mr-3"
                    >
                        Bezárás
                    </button>
                    <button
                        type="submit"
                        class="btn btn-success ml-3"
                        v-on:click="savePowerPlant"
                    >
                        Mentés
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

    data() {
        return {
            isEditing: false,
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
        };
    },

    mounted() {
        this.isEditing = "name" in this.powerPlant;
    },

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
.card {
    background-color: white;
    border: 1px solid #333;
    box-shadow: #333 0px 0px 3px;
    border-radius: 15px;
    padding: 1rem 0.5rem;
    margin: 1rem 0.5rem;
}

.faicon {
    cursor: pointer;
    vertical-align: center;
    padding: 0.5rem;
}

.white {
    color: white;
}

h5 {
    margin: 0;
}

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
    padding: 10px;
    border-radius: 15px;
    border: 3px solid #333;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    opacity: 1;
    max-width: 85%;
    max-height: 90%;
    z-index: 1000;
    text-align: center;
}

.modal-body {
    max-height: calc(90vh - 20px);
    overflow: auto;
    padding: 10px;
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
    max-width: 85%;
    z-index: 999;
}

.red {
    color: red;
}
</style>
