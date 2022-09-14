export const state = () => ({
    left: false,
    content: ''
  })
  
  export const getters = {}
  
  export const mutations = {
    SET_LEFT(state, value) {
      state.left = value
    },
    SET_CONTENT(state, content) {
        state.content = content
    }
  }
  
  export const actions = {
    async setLeftPanel({commit}, value) {
        await commit('SET_LEFT', value)
    },
    async setLeftContent({commit}, content) {
        await commit('SET_CONTENT', content)
        console.log('Left content: ', content)
    }
  }
