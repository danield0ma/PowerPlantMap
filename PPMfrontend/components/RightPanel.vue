<template>
    <div id="innerRight">
        <div style="display: flex;">
            <div style="display: inline; vertical-align: sub;">
                <img src="hu.png" alt="zászló" width="40px" height="20px" style="margin-top: 0.7rem;">
            </div>
            <h4 style="padding-left: 0.5rem; display: inline; vertical-align: top;">Hungary</h4>
        </div>
        <p style="padding: 0;">{{ startTime }} - {{ endTime }}</p>
        <h6>Teljes rendszerterhelés: {{ this.$store.state.power.currentLoad }} MW</h6>
        <h6>Energia-mix diagram</h6>
        <div>
            <client-only>
                <line-chart
                    :chart-data = "chartData()"
                    :chart-options = "chartOptions"
                    :height = "500"
                    :width = "500"
                    chart-id = 'Energiamix'
                />
            </client-only>
        </div>
    </div>
</template>

<script>
import moment from 'moment'

export default {
    computed: {
        startTime() {
            return moment(this.$store.state.power.currentLoadDateTime).format('YYYY.MM.DD HH:mm')
        },

        endTime() {
            return moment(this.$store.state.power.currentLoadDateTime).format('YYYY.MM.DD HH:mm')
        },

        chartOptions() {
            return {
                elements: {
                    line: {
                        borderColor: '#C1536D',
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
                        intersect: false
                    }
                },
                scales: {
                    y: {
                        min: 0,
                        grid: {
                            lineWidth: 0
                        },
                        stacked: true
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
        chartData() {
            return {
                labels: this.getDateArray(),
                datasets: [
                    {
                        label: 'Total System Load [MW]',
                        backgroundColor: '#777',
                        borderColor: '#777',
                        fill: false,
                        data: this.getLoadArray()
                    },
                    {
                        label: 'Paks [MW]',
                        backgroundColor: '#B7BF50',
                        borderColor: '#B7BF50',
                        pointRadius: 0,
                        stack: 'PP',
                        fill: {value: 0},
                        data: this.getPowerOfPowerPlant('PKS')
                    },
                    {
                        label: 'Mátra [MW]',
                        backgroundColor: '#B59C5E',
                        borderColor: '#B59C5E',
                        pointRadius: 0,
                        stack: 'PP',
                        fill: '-1',
                        data: this.getPowerOfPowerPlant('MTR')
                    },
                    {
                        label: 'Dunamenti [MW]',
                        backgroundColor: '#e691a5',
                        borderColor: '#e691a5',
                        stack: 'PP',
                        fill: '-1',
                        data: this.getPowerOfPowerPlant('DME')
                    },
                    {
                        label: 'Gönyű [MW]',
                        backgroundColor: '#C1536D',
                        borderColor: '#C1536D',
                        stack: 'PP',
                        fill: '-1',
                        data: this.getPowerOfPowerPlant('GNY')
                    },
                    {
                        label: 'Csepel II. [MW]',
                        backgroundColor: '#990f30',
                        borderColor: '#990f30',
                        stack: 'PP',
                        fill: '-1',
                        data: this.getPowerOfPowerPlant('CSP')
                    },
                    {
                        label: 'Kispest [MW]',
                        backgroundColor: '#5c0318',
                        borderColor: '#5c0318',
                        stack: 'PP',
                        fill: '-1',
                        data: this.getPowerOfPowerPlant('KIP')
                    }
                ]
            }
        },

        getDateArray() {
            let dateArray = []
            for (const load of this.$store.state.power.loadHistory)
            {
                dateArray.push(moment(load.end).format('HH:mm'))
            }

            return dateArray
        },

        getLoadArray() {
            let loadArray = []
            for (const load of this.$store.state.power.loadHistory)
            {
                loadArray.push(load.currentLoad)
            }

            return loadArray
        },

        getPowerOfPowerPlant(PPID) {
            let powerArray = this.$store.state.power.powerOfPowerPlants
            for (let power of powerArray)
            {
                if(power.powerPlantBloc == PPID)
                {
                    return power.power
                }
            }
        }
    }
}
</script>