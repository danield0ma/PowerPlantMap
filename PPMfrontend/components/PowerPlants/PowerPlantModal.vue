<template>
    <div>
        <div class="modal-backdrop" v-on:click="$emit('close')"></div>
        <div class="shade-content"></div>
        <div class="modal-content" v-on:click.stop>
            <div class="modal-body">
                <h1 v-if="this.isEditing">
                    {{ powerPlant.description }} adatainak szerkesztése
                </h1>
                <h1 v-else>Új erőmű létrehozása</h1>
                <div class="row text-left d-flex justify-content-around">
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
                            <label for="image">Kép:</label>
                            <input
                                type="text"
                                class="form-control"
                                id="image"
                                v-model="powerPlant.image"
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
                        <div class="form-group">
                            <label for="isCountry">Ország?</label>
                            <input
                                type="text"
                                class="form-control"
                                id="isCountry"
                                v-model="powerPlant.isCountry"
                                required
                            />
                        </div>
                    </div>
                    <div class="col-md-5 card">
                        <h5 class="text-center">Erőmű blokkjai</h5>
                        <div v-if="powerPlant.blocs !== '[]'">
                            <div
                                v-for="bloc in powerPlant.blocs"
                                :key="bloc.id"
                                class="card"
                            >
                                <div
                                    class="d-flex justify-content-between align-items-center"
                                >
                                    <h5 class="text-center flex-grow-1">
                                        {{ bloc.blocId }}
                                    </h5>
                                    <font-awesome-icon
                                        :icon="['fas', 'trash']"
                                        :size="'lg'"
                                        v-on:click="removeBloc(bloc)"
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
                                    :key="generator.id"
                                    class="card"
                                >
                                    <div
                                        class="d-flex justify-content-between align-items-center"
                                    >
                                        <h5 class="text-center flex-grow-1">
                                            {{ generator.generatorId }}
                                        </h5>
                                        <font-awesome-icon
                                            :icon="['fas', 'trash']"
                                            :size="'lg'"
                                            v-on:click="
                                                removeGenerator(bloc, generator)
                                            "
                                            class="faicon red"
                                        />
                                    </div>
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
                                </div>
                                <button
                                    class="btn btn-success d-flex justify-content-center align-items-center p-0 ml-2 mr-2"
                                    v-on:click="addGenerator(bloc)"
                                >
                                    Generátor hozzáadása
                                    <font-awesome-icon
                                        :icon="['fas', 'plus']"
                                        class="faicon white"
                                    />
                                </button>
                            </div>
                        </div>
                        <button
                            class="btn btn-success d-flex justify-content-center align-items-center p-0 ml-2 mr-2"
                            v-on:click="addBloc"
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
            // powerPlant: {},
        };
    },

    mounted() {
        this.editing = this.powerPlant.powerPlantId !== "";

        if (this.isEditing) {
            this.powerPlant.blocs.map((bloc, index) => {
                bloc.id = index;
                bloc.generators.map((generator, index) => {
                    generator.id = index;
                });
            });
        }
    },

    methods: {
        addBloc() {
            const newBloc = {
                id: this.powerPlant.blocs.length,
                blocId: "",
                blocType: "",
                commissionDate: "",
                maxBlocCapacity: "",
                generators: [],
            };

            if (this.powerPlant.blocs.length > 0) {
                this.powerPlant.blocs.push(newBloc);
            } else {
                this.powerPlant.blocs = [newBloc];
            }

            this.addGenerator(
                this.powerPlant.blocs[this.powerPlant.blocs.length - 1]
            );
        },

        removeBloc(bloc) {
            const index = this.powerPlant.blocs.indexOf(bloc);
            if (index > -1) {
                this.powerPlant.blocs.splice(index, 1);
            }
        },

        addGenerator(bloc) {
            bloc.generators.push({
                id: bloc.generators.length,
                generatorId: "",
                maxCapacity: "",
            });
        },

        removeGenerator(bloc, generator) {
            const index = bloc.generators.indexOf(generator);
            if (index > -1) {
                bloc.generators.splice(index, 1);
            }
        },

        async savePowerPlant() {
            console.log(this.powerPlant);
            await this.$axios.$post(
                "api/PowerPlant/AddPowerPlant",
                this.powerPlant
            );
        },
    },
};
</script>

<style scoped>
h5 {
    margin: 0;
}
</style>
