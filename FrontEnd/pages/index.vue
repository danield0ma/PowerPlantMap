<template>
	<div>
		<div style="height: 3.5rem; position: absolute"></div>
		<div id="left" v-if="showLeftPanel">
			<LeftPanel></LeftPanel>
		</div>
		<div id="rightPanel" v-if="rightNotLoading">
			<RightPanel :powerArray="powerOfPowerPlants" />
		</div>
		<div id="chooseDay">
			<p>Napválasztó</p>
			<input type="date" v-model="chosenDate" />
			<button
				v-on:click="setDate"
				class="btn btn-primary"
				style="margin-left: 0.5rem"
			>
				OK
			</button>
		</div>
		<div id="map"></div>
	</div>
</template>

<script>
import { usePowerStore } from "~/store/powerStore";
import { ref } from "vue";
import mapboxgl from "mapbox-gl";
import LeftPanel from "../components/PowerData/LeftPanel";
import RightPanel from "../components/PowerData/RightPanel";
import moment from "moment";

export default {
	name: "MapView",

	setup() {
		const powerStore = usePowerStore();

		const showLeftPanel = ref(powerStore.left);
		const chosenDate = ref(powerStore.date);
		const rightNotLoading = ref(!powerStore.rightLoading);
		const content = ref(powerStore.content);

		const gj = ref(null);
		const powerOfPowerPlants = ref(null);
		const BASE_PATH = "https://powerplantmap.tech:5001/";

		const {
			data: basicsResponse,
			error,
			pending,
			refresh,
		} = useFetch(`${BASE_PATH}API/PowerData/GetPowerPlantBasics`);

		const {
			data: powersResponse,
			error: error2,
			pending: pending2,
			refresh: refresh2,
		} = useFetch(`${BASE_PATH}API/PowerData/GetPowerOfPowerPlants`);

		watch(basicsResponse, (newBasics) => {
			if (newBasics) {
				gj.value = {
					type: "geojson",
					data: {
						type: "FeatureCollection",
						features: newBasics,
					},
				};
			}
		});

		watch(powersResponse, (newPowers) => {
			if (newPowers) {
				powerOfPowerPlants.value = newPowers;
			}
		});

		return {
			chosenDate,
			showLeftPanel,
			rightNotLoading,
			content,
			gj,
			powerOfPowerPlants,
		};
	},

	data() {
		return {
			accessToken: process.env.ACCESS_TOKEN,
			map: {},
			marker: [],
			popup: {},
			BASE_PATH: "https://powerplantmap.tech:5001/",
			// gj: {},
			// powerOfPowerPlants: {},
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

	// mounted() {
	// 	this.createMap();
	// 	// this.getLoad();
	// 	this.chosenDate = moment(Date(Date.now())).format("YYYY-MM-DD");
	// },

	// async asyncData() {
	// 	console.log("asyncData");
	// 	const BASE_PATH = "https://powerplantmap.tech:5001/";
	// 	const { basics, error, pending, refresh } = await useFetch(
	// 		`${BASE_PATH}API/PowerData/GetPowerPlantBasics`
	// 	);
	// 	if (!basics.ok) {
	// 		throw new Error(`HTTP error! status: ${basics.status}`);
	// 	}
	// 	const features = await basics.json();
	// 	console.log(features);
	// 	const gj = {
	// 		type: "geojson",
	// 		data: {
	// 			type: "FeatureCollection",
	// 			features,
	// 		},
	// 	};

	// 	console.log(gj);

	// 	const { powers, error2, pending2, refresh2 } = await useFetch(
	// 		`${BASE_PATH}API/PowerData/getPowerOfPowerPlants`
	// 	);
	// 	const powerOfPowerPlants = await powers.json();

	// 	return { gj, powerOfPowerPlants };
	// },

	methods: {
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

			// (async () => {
			// 	while (this.gj === null) {
			// 		await new Promise((resolve) => setTimeout(resolve, 100));
			// 	}
			// })();

			// if (this.gj !== null) {
			console.log(this.gj);
			for (const powerPlant of this.gj.data.features) {
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
			// }
		},

		// async getPowerPlantBasics() {
		// 	const res = await fetch(
		// 		`${this.BASE_PATH}API/PowerData/getPowerPlantBasics`
		// 	);
		// 	const features = await res.json();

		// 	const data = {
		// 		type: "geojson",
		// 		data: {
		// 			type: "FeatureCollection",
		// 			features,
		// 		},
		// 	};
		// 	return data;
		// },

		async getDetailsOfPowerPlant(id) {
			try {
				await this.$store.dispatch("power/setLeftPanelLoading", true);
				await this.$store.dispatch("power/toggleBlocs", false);
				await this.$store.dispatch("power/setSelectedBloc", -1);
				await this.$store.dispatch("power/setLeftPanel", true);

				const res =
					this.chosenDate === null
						? await fetch(
								`${this.BASE_PATH}API/PowerData/getDetailsOfPowerPlant?id=${id}`
						  )
						: await fetch(
								`${this.BASE_PATH}API/PowerData/getDetailsOfPowerPlant?id=${id}&date=${this.chosenDate}`
						  );
				const data = await res.json();
				await this.$store.dispatch("power/setLeftContent", data);
				await this.$store.dispatch("power/setLeftPanelLoading", false);
			} catch (error) {
				console.error(error);
			}
		},

		async setDate() {
			this.$store.dispatch("power/setRightLoading", true);
			this.$store.dispatch("power/setLeftPanel", false);
			await this.$store.dispatch("power/setDate", this.chosenDate);
			await this.getLoad();
		},
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
	width: 33vw;
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
	width: 33vw;
	margin-top: 3.5rem;
	right: 0;
}

#innerRight {
	padding: 0.5rem 1rem;
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
