import { defineStore } from "pinia";

export const usePowerStore = defineStore({
	id: "powerStore",

	state: () => ({
		date: null as Date | null,
		left: false,
		content: {},
		isLoading: false,
		rightLoading: true,
		enableBlocs: false,
		currentLoad: 0,
		selectedBloc: -1,
	}),

	getters: {},

	actions: {
		setDate(value: Date | null) {
			this.date = value;
		},

		setLeftPanel(value: boolean) {
			this.left = value;
		},

		setLeftContent(content: object) {
			this.content = content;
		},

		setLeftPanelLoading(value: boolean) {
			this.isLoading = value;
		},

		setRightLoading(value: boolean) {
			this.rightLoading = value;
		},

		toggleBlocs(value: boolean) {
			this.enableBlocs = value;
		},

		setSelectedBloc(value: number) {
			this.selectedBloc = value;
		},
	},
});
