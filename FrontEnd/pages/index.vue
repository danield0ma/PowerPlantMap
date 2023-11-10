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

	// setup() {
	// 	const BASE_PATH = "https://powerplantmap.tech:5001/";
	// 	const gj = ref(null);
	// 	const powerOfPowerPlants = ref(null);

	// 	const {
	// 		data: features,
	// 		error,
	// 		fetch,
	// 	} = useFetch(`${BASE_PATH}API/PowerData/GetPowerPlantBasics`);
	// 	if (!fetch) {
	// 		console.log(`feature: ${features}`);
	// 	}
	// 	gj.value = {
	// 		type: "geojson",
	// 		data: {
	// 			type: "FeatureCollection",
	// 			features,
	// 		},
	// 	};
	// 	console.log(`DATA: ${gj}`);
	// 	console.log(`DATA: ${gj.data}`);

	// 	useFetch(`${BASE_PATH}API/PowerData/GetPowerOfPowerPlants`).then((data) => {
	// 		powerOfPowerPlants.value = data;
	// 	});
	// 	console.log(`powers: ${powerOfPowerPlants.data}`);

	// 	return { gj, powerOfPowerPlants };
	// },

	data() {
		return {
			accessToken: process.env.ACCESS_TOKEN,
			map: {},
			marker: [],
			popup: {},
			BASE_PATH: "https://powerplantmap.tech:5001/",
			gj: [],
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

	async mounted() {
		console.log("mounted");
		await this.getData();
		this.createMap();
		// this.getLoad();
		this.chosenDate = moment(Date(Date.now())).format("YYYY-MM-DD");
	},

	computed: {
		showLeftPanel() {
			const powerStore = usePowerStore();
			return powerStore.left;
		},

		chosenDate() {
			const powerStore = usePowerStore();
			return powerStore.date;
		},

		rightNotLoading() {
			const powerStore = usePowerStore();
			return !powerStore.rightLoading;
		},
	},

	methods: {
		async getData() {
			console.log("asyncData");
			const BASE_PATH = "https://powerplantmap.tech:5001/";
			const res = await fetch(`${BASE_PATH}API/PowerData/GetPowerPlantBasics`);
			const basics = await res.json();
			console.log("basics: " + basics[0].type);
			this.gj = {
				type: "geojson",
				data: {
					type: "FeatureCollection",
					features: basics,
				},
			};

			const powersRes = await fetch(
				`${BASE_PATH}API/PowerData/GetPowerOfPowerPlants`
			);
			const powers = await powersRes.json();
			this.powerOfPowerPlants = powers;
			console.log("POWER: " + this.powerOfPowerPlants.data[0].powerPlantName);
			return;
		},

		createMap() {
			// if (this.gj === null) {
			// 	this.createMap();
			// }

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
			// 	while (!this.gj || !this.gj.data || !this.gj.data.features) {
			// 		await new Promise((resolve) => setTimeout(resolve, 100));
			// 	}
			// })();

			// if (this.gj.data !== undefined) {
			console.log("MAP: " + this.gj);
			const powerPlants = this.gj.data.features;
			console.log(powerPlants);
			for (const powerPlant of powerPlants) {
				console.log("powerplant " + powerPlant.properties.img);
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
