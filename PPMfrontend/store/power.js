export const state = () => ({
    left: false,
    content: '',
    isLoading: false
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
  }
  }
  
  export const actions = {
    async setLeftPanel({commit}, value) {
      await commit('SET_LEFT', value)
    },
    async setLeftContent({commit}, content) {
      await commit('SET_CONTENT', content)
      console.log('Left content: ', content)
    },
    async setLeftPanelLoading({commit}, value) {
      await commit('SET_LOADING', value)
    }
  }
