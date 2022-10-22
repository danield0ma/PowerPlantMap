<template>
    <div id="innerRight">
        <div style="display: flex;">
            <div style="display: inline; vertical-align: sub;">
                <img src="hu.png" alt="zászló" width="40px" height="20px" style="margin-top: 0.7rem;">
            </div>
            <h4 style="padding-left: 0.5rem; display: inline; vertical-align: top;">Hungary</h4>
        </div>
        <p style="padding: 0;">{{ time }}</p>
        <h6>Teljes rendszerterhelés: {{ this.$store.state.power.currentLoad }} MW</h6>
        <h6>Energia-mix diagram</h6>
        <!-- <div>
            <client-only>
                <line-chart
                    :chart-data = "chartData('all')"
                    :chart-options = "chartOptions"
                    :height = "300"
                    :width = "500"
                    chart-id = 'Energiamix'
                />
            </client-only>
        </div> -->
    </div>
</template>

<script>
import moment from 'moment'

export default {
    created() {
        this.getCurrentLoad()
    },

    computed: {
        time() {
            return moment(this.$store.state.power.currentLoadDateTime).format('YYYY.MM.DD HH:mm')
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
                        borderColor: this.color,
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
        },

        color() {
            return '#' + this.$store.state.power.content.color
        }
    },

    methods: {
        async getCurrentLoad() {
            const res = await fetch('https://localhost:7032/API/Power/getCurrentLoad/')
            const f = await res.json()
            this.$store.dispatch('power/setCurrentLoad', f)
        },

        chartData(blocID) {
            let asd = {
                labels: this.getDateArray(),
                datasets: [
                    {
                        label: 'Power [MW]',
                        backgroundColor: this.color,
                        data: this.getPowerArray(blocID)
                    }
                    // {
                    //     label: 'Max Capacity [MW]',
                    //     backgroundColor: '#777',
                    //     data: this.getMaxCap(blocID)
                    // }
                ]
            }
            return asd
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
            return resultArray
        },

        getPowerArray(blocID) {
            let choice = false
            if(blocID == 'all') {
                choice = true
            }
            
            let data = this.$store.state.power.content
            let a = []
            for(let i = 0; i < 96; i++)
            {
                a.push(0)
            }
            

            for(let bloc of data.blocs) {
                if(choice || blocID == bloc.blocID) {
                    for(let generator of bloc.generators) {
                        for(let i = 0; i < 96; i++) {
                            a[i] += generator.currentPower[i]
                        }
                    }
                }
            }
            return a
        }
    }
}
</script>