export const state = () => ({
    left: false,
    content: '',
    isLoading: false,
    enableBlocs: false,
    currentLoad: 0,
    currentLoadDateTime: ''
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
    TOGGLE_BLOCS(state, value) {
      state.enableBlocs = value
    },
    SET_CURRENTLOAD(state, value) {
      state.currentLoad = value
    },
    SET_CURRENTLOADDATETIME(state, value) {
      state.currentLoadDateTime = value
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
    async toggleBlocs({commit}, value) {
      await commit('TOGGLE_BLOCS', value)
    },
    async setCurrentLoad({commit}, value) {
      await commit('SET_CURRENTLOAD', value.currentLoad)
      await commit('SET_CURRENTLOADDATETIME', value.end)
    }
  }
