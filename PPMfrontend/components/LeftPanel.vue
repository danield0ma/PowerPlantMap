<template>
    <div>
        <div v-if="isLoading">
            <h1>LOADING...</h1>
        </div>
        <div v-else style="padding: 0.5rem 1rem;">
            <div class="flexbox">
                <h4>{{ content.description }}</h4>
                <div class="inline">
                    <font-awesome-icon icon="fa-solid fa-xmark fa-xs" 
                            class="faicon" v-on:click="closePanel"
                    />
                </div>
            </div>
            <h6>Cím: </h6>
            <h6>Kontakt: </h6>
            <h6>Üzemeltető: {{ content.operatorCompany }}</h6>
            <h6>Web: {{ content.webpage }}</h6>
            <!-- <a href={{ content.webpage }}>{{ content.webpage }}</a> -->
            <h6>Max teljesítmény: {{ content.maxPower }} MW</h6>

            <client-only>
                <line-chart
                    :chart-data = "chartData"
                    :chart-options = "chartOptions"
                    :height = "300"
                    :width = "500"
                    chart-id = bloc.blocID
                />
            </client-only>

            <div class="flexbox" v-if="content.blocs.length > 1">
                <h4>Blokkok</h4>
                <div class="inline">
                    <font-awesome-icon icon="fa-solid fa-xmark fa-xs" 
                            class="faicon" v-on:click="toggleBlocs"
                    />
                </div>
            </div>

            <div v-if="blocsEnabled">
                <div v-for="bloc in content.blocs" :key="bloc.blocID">
                    <div class="flexbox">
                        <h6>{{bloc.blocID}} ({{bloc.blocType}}): {{bloc.currentPower}}/{{bloc.maxBlocCapacity}}MW</h6>
                        <div class="inline" v-if="bloc.generators.length > 1">
                            <font-awesome-icon icon="fa-solid fa-xmark fa-xs" class="faicon" />
                        </div>
                    </div>
                    <client-only>
                        <line-chart
                            :chart-data = "chartData"
                            :chart-options = "chartOptions"
                            :height = "300"
                            :width = "500"
                            chart-id = bloc.blocID
                        />
                    </client-only>
                    
                    
                    <!-- <client-only>
                        <line-chart :data="chartData"></line-chart>
                    </client-only> -->
                    <div v-if="bloc.generators.length > 1">
                        <div v-for="generator in bloc.generators" :key="generator.generatorID">
                            <p>{{generator.generatorID}}: {{generator.currentPower[0]}}/{{generator.maxCapacity}}MW</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import moment from 'moment'

export default {
    name: 'LeftPanel',

    created() {
        moment.locale('hu')
    },

    computed: {
        isLoading() {
            return this.$store.state.power.isLoading
        },

        content() {
            while(this.isLoading) {
                pass
            }
            return this.$store.state.power.content
        },

        blocsEnabled() {
            return this.$store.state.power.enableBlocs
        },

        chartData() {
            let asd = {
                labels: this.getDateArray(),
                datasets: [
                    {
                        label: 'Power [MW]',
                        backgroundColor: '#C1536D',
                        data: this.getPowerArray()
                    },
                    {
                        label: 'Max Capacity [MW]',
                        backgroundColor: '#C1536D',
                        data: this.getMaxCap()
                    }
                ]
            }
            return asd
        },

        chartOptions() {
            return {
                plugins: {
                    title: {
                        display: false
                    },
                    legend: {
                        display: false
                    }
                },
                elements: {
                    line: {
                        borderColor: '#C1536D',
                        borderWidth: 2
                    }
                },
                layout: {
                    padding: 0
                },
                tooltips: {
                    enabled: true
                }
            }
        }
    },
    methods: {
        getContent() {
            while(this.isLoading) {
                console.log('getContent')
            }
            return this.$store.state.power.content
        },

        closePanel() {
            this.$store.dispatch('power/setLeftPanel', false)
        },

        getDateArray() {
            moment.locale('hu')
            //console.log('DataStart: ')
            console.log(this.$store.state.power.content.dataStart)
            let time = moment(this.$store.state.power.content.dataStart).toDate()
            console.log(moment(time).format("hh:mm"))
            
            let timeArray = []
            let resultArray = []
            let previous = time
            timeArray.push(time)
            resultArray.push(moment(time).format("hh:mm"))
            for(let i=1; i<96; i++)
            {
                let time = moment(previous).add(15, 'm').toDate()
                timeArray.push(time)
                resultArray.push(moment(time).format("hh:mm"))
                previous = time
            }
            console.log(resultArray)
            
            return resultArray
        },

        getPowerArray() {
            let data = this.getContent()
            let a = JSON.parse(JSON.stringify(data.blocs[0].generators[0].currentPower))
            for(let i=a.length; i<96; i++)
            {
                a.push(0)
            }
            console.log(a)
            return a
        },

        getMaxCap() {
            let arr = []
            for(let i=0;i<96;i++)
            {
                arr.push(433)
            }
            return arr
        },

        async toggleBlocs() {
            if(this.blocsEnabled) {
                await this.$store.dispatch('power/toggleBlocs', false)
            } else {
                await this.$store.dispatch('power/toggleBlocs', true)
            }
        }
    }
}
</script>

<style>
    p {
        margin: 0;
        padding: 0 0 0.5rem 1rem;
    }

    h4 {
        margin: 0.3rem 0;
    }

    h6 {
        margin: 0.3rem 0;
    }

    .inline {
        display: inline;
    }

    .flexbox {
        display: flex;
        justify-content: space-between;
    }

    .faicon {
        cursor: pointer;
        color: red;
        vertical-align: sub;
    }
</style>