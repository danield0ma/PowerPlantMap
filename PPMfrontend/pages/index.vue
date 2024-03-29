<template>
    <div>
        <div style="height: 3.5rem; position: absolute"></div>
        <div id="leftPanel" v-if="showLeftPanel" class="col-md-4 overflow-auto">
            <LeftPanel></LeftPanel>
        </div>
        <div
            id="rightPanel"
            v-if="showRightPanel"
            class="col-md-4 overflow-auto"
        >
            <RightPanel
                :powerOfPowerPlants="powerOfPowerPlants"
                :basicsOfPowerPlants="gj.data.features.map((x) => x.properties)"
                @changeDate="changeDate"
            />
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
                top: 6rem;
                z-index: 500;
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
        if (this.getDate === null || this.getDate === undefined) {
            this.$store.dispatch(
                "power/setDate",
                moment(Date(Date.now())).format("YYYY-MM-DD")
            );
        }
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
            return this.$store.state.power.right;
        },

        isRightPanelLoading() {
            return this.$store.state.power.rightLoading;
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
            if (
                this.getDate !== null &&
                this.getDate !== undefined &&
                this.getDate !== moment(Date(Date.now())).format("YYYY-MM-DD")
            ) {
                this.powerOfPowerPlants = await this.$axios.$get(
                    `api/PowerData/getPowerOfPowerPlants?date=${this.getDate}`
                );
            }
            await this.$store.dispatch("power/setRightLoading", false);
        },

        createMap() {
            this.map = new mapboxgl.Map({
                accessToken: process.env.ACCESS_TOKEN,
                container: "map",
                style: process.env.STYLE,
                center: [19.7, 47.15],
                zoom: 6.5,
                maxZoom: 7.99,
                minZoom: 4.5,
            });

            const powerPlants = this.gj.data.features;
            for (const powerPlant of powerPlants) {
                const element = document.createElement("div");
                element.className = "markerElement";
                element.style.backgroundImage = `url('${powerPlant.properties.img}')`;

                const marker = new mapboxgl.Marker(element)
                    .setLngLat(powerPlant.geometry.coordinates)
                    .addTo(this.map);

                marker.getElement().addEventListener("click", () => {
                    if (
                        this.showLeftPanel &&
                        this.content.powerPlantId === powerPlant.properties.id
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
                    this.getDate === null ||
                    this.getDate === undefined ||
                    this.getDate ===
                        moment(Date(Date.now())).format("YYYY-MM-DD")
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

        async changeDate() {
            this.$store.dispatch("power/setRightLoading", true);
            this.$store.dispatch("power/setLeftPanel", false);
            await this.getLoad();
            await this.$store.dispatch("power/setRightLoading", false);
        },
    },
};
</script>

<style scoped>
#map {
    width: 100vw;
    height: 100vh;
    position: relative;
}

#rightPanel {
    display: block;
    position: absolute;
    z-index: 499;
    background: rgba(255, 255, 255, 0.75);
    height: calc(100vh - 3.5rem);
    margin-top: 3.5rem;
    right: 0;
}

#leftPanel {
    display: block;
    position: absolute;
    z-index: 501;
    background: rgba(255, 255, 255, 0.75);
    height: auto;
    margin-top: 3.5rem;
    bottom: 0;
    top: 0;
}

@media (max-width: 768px) {
    #rightPanel {
        background: rgba(255, 255, 255);
    }

    #leftPanel {
        background: rgba(255, 255, 255);
    }
}

.flexbox {
    display: flex;
    justify-content: space-between;
}
</style>
