export const state = () => ({
    date: null,
    left: false,
    right: true,
    content: "",
    isLoading: false,
    rightLoading: true,
    enableBlocs: false,
    currentLoad: 0,
    selectedBloc: -1,
});

export const getters = {};

export const mutations = {
    SET_DATE(state, value) {
        state.date = value;
    },
    SET_LEFT(state, value) {
        state.left = value;
    },
    SET_RIGHT(state, value) {
        state.right = value;
    },
    SET_CONTENT(state, content) {
        state.content = content;
    },
    SET_LOADING(state, value) {
        state.isLoading = value;
    },
    SET_RIGHTLOADING(state, value) {
        state.rightLoading = value;
    },
    TOGGLE_BLOCS(state, value) {
        state.enableBlocs = value;
    },
    SET_CURRENTLOAD(state, value) {
        state.currentLoad = value;
    },
    SET_SELECTEDBLOC(state, value) {
        state.selectedBloc = value;
    },
};

export const actions = {
    async setDate({ commit }, value) {
        await commit("SET_DATE", value);
    },
    async setLeftPanel({ commit }, value) {
        await commit("SET_LEFT", value);
    },
    async setRightPanel({ commit }, value) {
        await commit("SET_RIGHT", value);
    },
    async setLeftContent({ commit }, content) {
        await commit("SET_CONTENT", content);
    },
    async setLeftPanelLoading({ commit }, value) {
        await commit("SET_LOADING", value);
    },
    async setRightLoading({ commit }, value) {
        await commit("SET_RIGHTLOADING", value);
    },
    async toggleBlocs({ commit }, value) {
        await commit("TOGGLE_BLOCS", value);
    },
    async setSelectedBloc({ commit }, value) {
        await commit("SET_SELECTEDBLOC", value);
    },
};
