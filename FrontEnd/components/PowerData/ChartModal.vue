<template>
    <div>
        <div class="modal-backdrop" v-on:click="$emit('close')"></div>
        <div class="shade-content"></div>
        <div class="modal-content" v-on:click.stop>
            <div class="modal-body">
                <h1>Scrollable chart</h1>
                <!-- <div ref="chart"></div> -->
            </div>
        </div>
    </div>
</template>

<script>
// import Highcharts from "highcharts";
export default {
    name: "ChartModal",
    props: {
        powerArray: {
            type: Object,
            required: true,
        },
    },

    data() {
        return {
            chartOptions: {
                chart: {
                    type: "spline",
                },
                title: {
                    text: "Entire title",
                },
                series: [
                    {
                        data: [10, 0, 8, 2, 6, 4, 5, 5],
                    },
                ],
            },
        };
    },

    // mounted() {
    //     if (process.client) {
    //         import("highcharts").then((Highcharts) => {
    //             // Create the chart
    //             Highcharts.chart(this.$refs.chart, {
    //                 // Highcharts configuration options
    //                 // ...
    //             });
    //         });
    //     }
    // },

    methods: {},
};
</script>

<style>
.modal-backdrop {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgb(0, 0, 0);
    opacity: 0.75 !important;
    z-index: 998;
}

.modal-content {
    background-color: #f5f5f5;
    color: #000;
    padding: 10px;
    border-radius: 15px;
    border: 3px solid #333;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    opacity: 1;
    max-width: 85%;
    max-height: 90%;
    z-index: 1000;
    text-align: center;
}

.modal-body {
    max-height: calc(90vh - 20px);
    overflow: auto;
    overflow-x: hidden;
    padding: 10px;
}

.shade-content {
    background-color: #000;
    opacity: 1;
    border-radius: 15px;
    border: 3px solid #333;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    max-width: 85%;
    z-index: 999;
}
</style>
