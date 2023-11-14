<template>
    <div>
        <div style="height: 3.5rem; position: absolute"></div>
        <div id="left" v-if="showLeftPanel" class="col-md-4">
            <LeftPanel></LeftPanel>
        </div>
        <div id="rightPanel" v-if="showRightPanel" class="col-md-4">
            <RightPanel :powerArray="powerOfPowerPlants" />
        </div>
        <font-awesome-icon
            v-else
            v-on:click="openRightPanel"
            icon="fa-solid fa-square-plus"
            class="faicon"
            :size="'3x'"
            style="
                position: absolute;
                right: 1rem;
                top: 4rem;
                z-index: 2000;
                color: green;
            "
        />
        <div id="map"></div>
    </div>
</template>

<script>
import mapboxgl from "mapbox-gl";
import LeftPanel from "../components/PowerData/LeftPanel";
import RightPanel from "../components/PowerData/RightPanel";
import moment from "moment";

export default {
    name: "MapView",
    data() {
        return {
            map: {},
            gj: {},
            marker: [],
            popup: {},
            powerOfPowerPlants: {},
        };
    },

    head() {
        return {
            title: "Map View - PowerPlantMap",
        };
    },

    components: {
        LeftPanel,
        RightPanel,
    },

    mounted() {
        this.createMap();
        this.getLoad();
    },

    async asyncData({ $axios }) {
        const features = await $axios.$get(
            `/api/PowerData/getPowerPlantBasics`
        );
        const gj = {
            type: "geojson",
            data: {
                type: "FeatureCollection",
                features,
            },
        };

        const powerOfPowerPlants = await $axios.$get(
            `/api/PowerData/getPowerOfPowerPlants`
        );

        return { gj, powerOfPowerPlants };
    },

    computed: {
        showLeftPanel() {
            return this.$store.state.power.left;
        },

        showRightPanel() {
            return (
                !this.$store.state.power.rightLoading &&
                this.$store.state.power.right
            );
        },

        content() {
            return this.$store.state.power.content;
        },

        getDate() {
            return this.$store.state.power.date;
        },
    },

    methods: {
        openRightPanel() {
            this.$store.dispatch("power/setRightPanel", true);
        },

        async getLoad() {
            if (this.getDate != null && this.getDate != undefined) {
                this.powerOfPowerPlants = await this.$axios.$get(
                    `api/PowerData/getPowerOfPowerPlants?date=${this.getDate}`
                );
            }
            await this.$store.dispatch("power/setRightLoading", false);
        },

        createMap() {
            this.map = new mapboxgl.Map({
                accessToken:
                    "pk.eyJ1IjoiZGFuaWVsZG9tYSIsImEiOiJjbDJvdDI1Mm4xNWZoM2NydWdxbWdvd3ViIn0.5x6xp0dGOMB_eh6_r_V79Q",
                container: "map",
                style: "mapbox://styles/danieldoma/cl6gnh6eg008l14pdjazw50fy",
                center: [19.7, 47.15],
                zoom: 6.75,
                maxZoom: 9,
                minZoom: 5,
            });

            const powerPlants = this.gj.data.features;
            for (const powerPlant of powerPlants) {
                const element = document.createElement("div");
                element.className = "marker";
                element.style.backgroundImage = `url('${powerPlant.properties.img}')`;
                element.style.width = "3rem";
                element.style.height = "3rem";
                element.style.backgroundSize = "100%";

                const marker = new mapboxgl.Marker(element)
                    .setLngLat(powerPlant.geometry.coordinates)
                    .addTo(this.map);

                marker.getElement().addEventListener("click", () => {
                    if (
                        this.showLeftPanel &&
                        this.content.powerPlantID === powerPlant.properties.id
                    ) {
                        this.$store.dispatch("power/setLeftPanel", false);
                        this.$store.dispatch("power/setSelectedBloc", -1);
                        this.$store.dispatch("power/toggleBlocs", false);
                    } else {
                        this.getDetailsOfPowerPlant(powerPlant.properties.id);
                    }
                });
            }
        },

        async getDetailsOfPowerPlant(id) {
            try {
                await this.$store.dispatch("power/setLeftPanelLoading", true);
                await this.$store.dispatch("power/toggleBlocs", false);
                await this.$store.dispatch("power/setSelectedBloc", -1);
                await this.$store.dispatch("power/setLeftPanel", true);

                const data =
                    this.getDate === null || this.getDate === undefined
                        ? await this.$axios.$get(
                              `/api/PowerData/getDetailsOfPowerPlant?id=${id}`
                          )
                        : await this.$axios.$get(
                              `/api/PowerData/getDetailsOfPowerPlant?id=${id}&date=${this.getDate}`
                          );
                await this.$store.dispatch("power/setLeftContent", data);
                await this.$store.dispatch("power/setLeftPanelLoading", false);
            } catch (error) {
                console.error(error);
            }
        },

        // async setDate() {
        //     this.$store.dispatch("power/setRightLoading", true);
        //     this.$store.dispatch("power/setLeftPanel", false);
        //     await this.$store.dispatch("power/setDate", this.chosenDate);
        //     await this.getLoad();
        // },
    },
};
</script>

<style>
body {
    margin: 0;
    padding: 0;
}

#left {
    display: block;
    position: absolute;
    z-index: 1;
    background: rgba(255, 255, 255, 0.75);
    height: auto;
    /* width: 33vw; */
    margin-top: 3.5rem;
    bottom: 0;
    top: 0;
    /* left:0;
    right:0; */
}

#map {
    width: 100vw;
    height: 100vh;
    position: relative;
}

.marker {
    display: block;
    border: none;
    /* border-radius: 50%; */
    cursor: pointer;
    padding: 0;
}

#rightPanel {
    /* float: right; */
    display: block;
    position: absolute;
    z-index: 1;
    background: rgba(255, 255, 255, 0.75);
    height: calc(100vh - 3.5rem);
    /* height: auto; */
    /* width: 33vw; */
    margin-top: 3.5rem;
    right: 0;
}

#innerRight {
    /* padding: 0.5rem 1rem; */
}

.flexbox {
    display: flex;
    justify-content: space-between;
}

#chooseDay {
    margin: auto;
    /* display: block; */
    position: absolute;
    z-index: 1;
    background: rgba(255, 255, 255, 0.75);
    /* height: calc(100vh - 3.5rem); */
    width: 20vw;
    height: 80px;
    text-align: center;
    bottom: 2rem;
    left: 40vw;
    border-radius: 25px;
}
</style>
