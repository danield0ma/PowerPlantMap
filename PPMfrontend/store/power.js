export const state = () => ({
    left: false,
    content: '',
    isLoading: false,
    rightLoading: true,
    enableBlocs: false,
    currentLoad: 0,
    currentLoadDateTime: '',
    loadHistory: [],
    powerOfPowerPlants: [],
    selectedBloc: -1
  })
  
  export const getters = {}
  
  export const mutations = {
    SET_LEFT(state, value) {
      state.left = value
    },
    SET_CONTENT(state, content) {
      state.content = content
    },
    SET_LOADING(state, value) {
      state.isLoading = value
    },
    SET_RIGHTLOADING(state, value) {
      state.rightLoading = value
    },
    TOGGLE_BLOCS(state, value) {
      state.enableBlocs = value
    },
    SET_CURRENTLOAD(state, value) {
      state.currentLoad = value
    },
    SET_CURRENTLOADDATETIME(state, value) {
      state.currentLoadDateTime = value
    },
    SET_LOADHISTORY(state, value) {
      state.loadHistory = value
    },
    SET_POWEROFPOWERPLANTS(state, value) {
      state.powerOfPowerPlants = value
    },
    SET_SELECTEDBLOC(state, value) {
      state.selectedBloc = value
    }
  }
  
  export const actions = {
    async setLeftPanel({commit}, value) {
      await commit('SET_LEFT', value)
    },
    async setLeftContent({commit}, content) {
      await commit('SET_CONTENT', content)
      //console.log('Left content: ', content)
    },
    async setLeftPanelLoading({commit}, value) {
      await commit('SET_LOADING', value)
    },
    async setRightLoading({commit}, value) {
      await commit('SET_RIGHTLOADING', value)
    },
    async toggleBlocs({commit}, value) {
      await commit('TOGGLE_BLOCS', value)
    },
    async setCurrentLoad({commit}, value) {
      await commit('SET_CURRENTLOAD', value.currentLoad)
      await commit('SET_CURRENTLOADDATETIME', value.end)
    },
    async setLoadHistory({commit}, value) {
      await commit('SET_LOADHISTORY', value)
    },
    async setPowerOfPowerPlants({commit}, value) {
      await commit('SET_POWEROFPOWERPLANTS', value)
    },
    async setSelectedBloc({commit}, value) {
      await commit('SET_SELECTEDBLOC', value)
    }
  }
