<template>
    <div>
        <div v-if="isLoading">
            <h1>LOADING...</h1>
        </div>
        <div v-else id="velse">
            <div class="flexbox">
                <h4>{{ content.description }}</h4>
                <div class="inline">
                    <font-awesome-icon icon="fa-solid fa-xmark fa-xs" 
                            class="faicon" v-on:click="closePanel"
                    />
                </div>
            </div>
            <p style="padding: 0;">{{ dataEnd }}</p>
            <h6>Cím: {{ content.address }}</h6>
            <h6>Üzemeltető: {{ content.operatorCompany }}</h6>
            <h6><a :href=content.webpage target="_blank">Weboldal</a></h6>
            <!-- <a href={{ content.webpage }}>{{ content.webpage }}</a> -->
            <h6>Max teljesítmény: {{ content.maxPower }} MW</h6>

            <div v-if="blocsNotEnabled">
                <client-only>
                    <line-chart
                        :chart-data = "chartData('all')"
                        :chart-options = "chartOptions"
                        :height = "300"
                        :width = "500"
                        chart-id = bloc.blocID
                    />
                </client-only>
            </div>

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
                            :chart-data = "chartData(bloc.blocID)"
                            :chart-options = "chartOptions"
                            :height = "300"
                            :width = "500"
                            chart-id = bloc.blocID
                        />
                    </client-only>

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
        dataEnd() {
            return moment(this.$store.state.power.content.dataEnd).format('YYYY.MM.DD HH:mm')
        },

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

        blocsNotEnabled() {
            return !this.$store.state.power.enableBlocs
        },

        color() {
            return '#' + this.content.color
        },

        chartOptions() {
            return {
                elements: {
                    line: {
                        borderWidth: 3
                    },
                    point: {
                        pointRadius: 0
                    }
                },
                layout: {
                    padding: 0
                },
                tooltips: {
                    enabled: true
                },
                plugins: {
                    title: {
                        display: false
                    },
                    legend: {
                        display: false
                    },
                    tooltip: {
                        intersect: false,
                        mode: 'nearest'
                    }
                },
                scales: {
                    y: {
                        min: 0,
                        grid: {
                            lineWidth: 0
                        },
                        stacked: false
                    },
                    x: {
                        grid: {
                            lineWidth: 0
                        }
                    }
                }
            }
        }
    },
    methods: {
        chartData(blocID) {
            let asd = {
                labels: this.getDateArray(),
                datasets: [
                    {
                        label: 'Power [MW]',
                        backgroundColor: this.color,
                        borderColor: this.color,
                        fill: {value: 0},
                        data: this.getPowerArray(blocID)
                    },
                    {
                        label: 'Max Capacity [MW]',
                        backgroundColor: '#777',
                        borderColor: '#777',
                        fill: false,
                        data: this.getMaxCap(blocID)
                    }
                ]
            }
            return asd
        },

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
            console.log(this.$store.state.power.content.dataStart)
            let time = moment(this.$store.state.power.content.dataStart).add(15, 'm').toDate()
            console.log(moment(time).format("HH:mm"))
            
            let timeArray = []
            let resultArray = []
            let previous = time
            timeArray.push(time)
            resultArray.push(moment(time).format("HH:mm"))
            for(let i=1; i<97; i++)
            {
                let time = moment(previous).add(15, 'm').toDate()
                timeArray.push(time)
                resultArray.push(moment(time).format("HH:mm"))
                previous = time
            }            
            return resultArray
        },

        getPowerArray(blocID) {
            let choice = false
            if(blocID == 'all') {
                choice = true
            }
            
            let data = this.content
            let a = []
            for(let i = 0; i < 97; i++)
            {
                a.push(0)
            }
            

            for(let bloc of data.blocs) {
                if(choice || blocID == bloc.blocID) {
                    for(let generator of bloc.generators) {
                        for(let i = 0; i < 97; i++) {
                            a[i] += generator.currentPower[i]
                        }
                    }
                }
            }
            return a
        },

        getMaxCap(blocID) {
            let choice = false
            if(blocID == 'all') {
                choice = true
            }
            let maxCap = 0

            for(let bloc of this.content.blocs) {
                if(choice || blocID == bloc.blocID) {
                    for(let generator of bloc.generators) {
                        maxCap += generator.maxCapacity
                    }
                }
            }

            let arr = []
            for(let i=0;i<97;i++)
            {
                arr.push(maxCap)
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

    #velse {
        max-height: calc(100vh - 3.5rem);
        padding: 0.5rem 1rem;
        overflow: auto;
    }
</style>