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
            <h6>Üzemeltető: {{ content.operatorCompany }}</h6>
            <h6>Web: {{ content.webpage }}</h6>
            <!-- <a href={{ content.webpage }}>{{ content.webpage }}</a> -->
            <h6>Max teljesítmény: {{ content.maxPower }} MW</h6>
            <h4>Blokkok</h4>
            <p>{{content.dataStart}} - {{content.dataEnd}}</p>
            <div v-for="bloc in content.blocs" :key="bloc.blocID">
                <div class="flexbox">
                    <h6>{{bloc.blocID}} ({{bloc.blocType}}): {{bloc.currentPower}}/{{bloc.maxBlocCapacity}}MW</h6>
                    <div class="inline" v-if="bloc.generators.length > 1">
                        <font-awesome-icon icon="fa-solid fa-xmark fa-xs" class="faicon" />
                    </div>
                </div>
                <line-chart
                    :chart-options = 'chartOptions'
                    :chart-data = 'chartData'
                    chart-id = 'Chart'
                />
                <div v-if="bloc.generators.length > 1">
                    <div v-for="generator in bloc.generators" :key="generator.generatorID">
                        <p>{{generator.generatorID}}: {{generator.currentPower[0]}}/{{generator.maxCapacity}}MW</p>
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
    // props: {
    //     content: {}
    // },
    data() {
        return {
            chartData: {
                    labels: this.getDateArray(),
                    datasets: [
                        {
                            //label: 'Data One',
                            backgroundColor: '#777',
                            data: [40, 20, 12]
                        }
                    ]
                },
            chartOptions: {
                title: {
                    display:true,
                    text:'Largest Cities In Massachusetts',
                    fontSize:25
                },
                legend:{
                    display:false,
                    position:'right',
                    labels:{
                        fontColor:'#000'
                    }
                },
                layout:{
                    padding: {
                        left:50,
                        right:0,
                        bottom:0,
                        top:0
                    }
                },
                tooltips: {
                    enabled:true
                }
            }
        }
    },
    computed: {
        isLoading() {
            return this.$store.state.power.isLoading
        },
        content() {
            return this.$store.state.power.content
        },
    },
    methods: {
        closePanel() {
            this.$emit('close')
        },

        getDateArray() {
            console.log('DataStart: ')
            console.log(this.content)
            return ['asd', 'asd', 'asd']
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