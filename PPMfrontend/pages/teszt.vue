<template>
    <div width="500px" height="300px">
        <line-chart
            :chart-options='chartOptions'
            :chart-data='chartData'
            chart-id='myCustomId'
        />
    </div>
</template>

<script>
export default {
    data() {
        return {
            chartData: {
                    labels: [ 'January', 'February', 'March'],
                    datasets: [
                        {
                            label: 'Data One',
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
                    display:true,
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
    }
}
</script>
