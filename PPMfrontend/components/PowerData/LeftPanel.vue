<template>
    <div>
        <div v-if="isLoading">
            <h1>LOADING...</h1>
        </div>
        <div v-else Id="velse">
            <div class="flexbox">
                <h4>{{ content.description }}</h4>
                <div class="inline">
                    <font-awesome-icon
                        icon="fa-solid fa-xmark fa-xs"
                        class="faicon red"
                        v-on:click="closePanel"
                        :size="'lg'"
                    />
                </div>
            </div>
            <p class="p-0 text-left">{{ dataStart }} - {{ dataEnd }}</p>
            <h6>Cím: {{ content.address }}</h6>
            <h6>
                GPS-koordináták: {{ content.longitude }}, {{ content.latitude }}
            </h6>
            <h6>Üzemeltető: {{ content.operatorCompany }}</h6>
            <h6>
                Weboldal:
                <a
                    :href="content.webpage"
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    {{ content.webpage }}
                </a>
            </h6>
            <h6>Max teljesítmény: {{ content.maxPower }} MW</h6>

            <div v-if="blocsNotEnabled">
                <client-only>
                    <line-chart
                        :chart-data="chartData('all', false)"
                        :chart-options="
                            chartOptions(
                                content.description + ' termelése',
                                'all',
                                false
                            )
                        "
                        :height="300"
                        :width="500"
                        chart-id="Teljes"
                    />
                </client-only>
            </div>

            <div
                class="flexbox"
                v-if="
                    content.blocs.length > 1 ||
                    content.blocs[0].generators.length > 1
                "
            >
                <h4>Blokkok</h4>
                <div class="inline">
                    <div v-if="blocsEnabled">
                        <font-awesome-icon
                            icon="fa-solid fa-minus fa-xl"
                            class="faicon red"
                            v-on:click="toggleBlocs"
                            :size="'lg'"
                        />
                    </div>
                    <div v-else>
                        <font-awesome-icon
                            icon="fa-solid fa-plus fa-xl"
                            class="faicon green"
                            v-on:click="toggleBlocs"
                            :size="'lg'"
                        />
                    </div>
                </div>
            </div>

            <div v-if="blocsEnabled">
                <client-only>
                    <line-chart
                        :chart-data="
                            chartData(content.blocs[selectedBloc].blocId, false)
                        "
                        :chart-options="
                            chartOptions(
                                content.blocs[selectedBloc].blocId +
                                    ' termelése',
                                content.blocs[selectedBloc].blocId,
                                false
                            )
                        "
                        :height="300"
                        :width="500"
                        chart-id="bloc"
                    />
                </client-only>

                <div
                    class="flexbox"
                    style="padding: 0 5rem; justify-content: space-evenly"
                >
                    <div
                        v-for="(bloc, index) in content.blocs"
                        :key="bloc.blocId"
                    >
                        <button
                            class="blocSelectionButton"
                            v-on:click="selectBloc(index)"
                        >
                            {{ index + 1 }}
                        </button>
                    </div>
                </div>

                <div v-if="content.blocs[selectedBloc].generators.length > 1">
                    <div class="flexbox" style="justify-content: space-around">
                        <div
                            v-for="generator in content.blocs[selectedBloc]
                                .generators"
                            :key="generator.generatorId"
                        >
                            <line-chart
                                :chart-data="
                                    chartData(generator.generatorId, true)
                                "
                                :chart-options="
                                    chartOptions(
                                        generator.generatorId,
                                        generator.generatorId,
                                        true
                                    )
                                "
                                :height="150"
                                :width="200"
                                chart-id="bloc"
                            />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import moment from "moment";

export default {
    name: "DetailsPanel",

    data() {
        return {
            powerArrayToDisplay: [],
        };
    },

    computed: {
        dataEnd() {
            return moment(this.$store.state.power.content.dataEnd).format(
                "YYYY.MM.DD HH:mm"
            );
        },

        dataStart() {
            return moment(this.$store.state.power.content.dataStart).format(
                "YYYY.MM.DD HH:mm"
            );
        },

        isLoading() {
            return this.$store.state.power.isLoading;
        },

        content() {
            while (this.isLoading) {
                pass;
            }
            return this.$store.state.power.content;
        },

        blocsEnabled() {
            return this.$store.state.power.enableBlocs;
        },

        blocsNotEnabled() {
            return !this.$store.state.power.enableBlocs;
        },

        color() {
            return `#${this.content.color}`;
        },

        selectedBloc() {
            return this.$store.state.power.selectedBloc;
        },
    },
    methods: {
        waitForVariableChange(targetVariable, targetValue) {
            return new Promise((resolve) => {
                const checkVariable = () => {
                    if (targetVariable === targetValue) {
                        resolve();
                    } else {
                        setTimeout(checkVariable, 100);
                    }
                };
                checkVariable();
            });
        },

        chartOptions(title, Id, isGenerator) {
            return {
                elements: {
                    line: {
                        borderWidth: 3,
                    },
                    point: {
                        pointRadius: 0,
                    },
                },
                layout: {
                    padding: 0,
                },
                tooltips: {
                    enabled: true,
                },
                plugins: {
                    title: {
                        display: true,
                        text: title,
                        labels: {
                            font: {
                                size: 20,
                            },
                        },
                    },
                    legend: {
                        display: false,
                    },
                    tooltip: {
                        intersect: false,
                        mode: "nearest",
                    },
                },
                scales: {
                    y: {
                        min: this.getMin(Id, isGenerator),
                        max: this.getMax(Id, isGenerator),
                        grid: {
                            lineWidth: 0,
                        },
                        stacked: false,
                    },
                    x: {
                        grid: {
                            lineWidth: 0,
                        },
                    },
                },
            };
        },

        chartData(blocId, isGenerator) {
            return {
                labels: this.getDateArray(),
                datasets: [
                    {
                        label: "Power [MW]",
                        backgroundColor: this.color,
                        borderColor: this.color,
                        fill: { value: 0 },
                        data: this.getPowerArray(blocId, isGenerator),
                    },
                    {
                        label: "Max Capacity [MW]",
                        backgroundColor: "#777",
                        borderColor: "#777",
                        fill: false,
                        data: this.getMaxCap(blocId),
                    },
                ],
            };
        },

        getContent() {
            return this.$store.state.power.content;
        },

        closePanel() {
            this.$store.dispatch("power/setLeftPanel", false);
        },

        getDateArray() {
            return this.content.blocs[0].generators[0].pastPower.map((x) =>
                moment(x.timePoint).format("HH:mm")
            );
        },

        getPowerArray(Id, isGenerator) {
            const powerArray = [];
            for (let i = 0; i < 96; i++) {
                powerArray.push(0);
            }

            for (const bloc of this.content.blocs) {
                if (Id === "all" || Id === bloc.blocId || isGenerator) {
                    for (let generator of bloc.generators) {
                        if (
                            !isGenerator ||
                            (isGenerator && Id === generator.generatorId)
                        ) {
                            const pastPower = generator.pastPower.map(
                                (x) => x.power
                            );
                            powerArray.forEach((value, index) => {
                                powerArray[index] += pastPower[index];
                            });
                        }
                    }
                }
            }
            return powerArray;
        },

        getMaxCap(blocId) {
            let maxCap = 0;
            for (const bloc of this.content.blocs) {
                if (blocId === "all" || blocId === bloc.blocId) {
                    for (const generator of bloc.generators) {
                        maxCap += generator.maxCapacity;
                    }
                }
            }

            const arr = [];
            for (let i = 0; i < 96; i++) {
                arr.push(maxCap);
            }
            return arr;
        },

        async toggleBlocs() {
            if (this.blocsEnabled) {
                await this.$store.dispatch("power/toggleBlocs", false);
                await this.$store.dispatch("power/setSelectedBloc", -1);
            } else {
                await this.$store.dispatch("power/setSelectedBloc", 0);
                await this.$store.dispatch("power/toggleBlocs", true);
            }
        },

        getMin(Id, isGenerator) {
            let min = Math.min(...this.getPowerArray(Id, isGenerator));
            if (min > 100) {
                min -= 100;
            }
            return Math.floor(min / 100) * 100;
        },

        getMax(Id, isGenerator) {
            const powerArray = this.getPowerArray(Id, isGenerator);
            if (
                powerArray.reduce(
                    (accumulator, currentValue) => accumulator + currentValue
                ) === 0
            ) {
                return 100;
            }
            const max = Math.max(...powerArray);
            return Math.ceil(max / 100) * 100;
        },

        async selectBloc(n) {
            await this.$store.dispatch("power/setSelectedBloc", n);
        },
    },
};
</script>

<style scoped>
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

#content {
    max-height: calc(100vh - 3.5rem);
    padding-left: 1rem;
    overflow: auto;
}

@media (max-width: 768px) {
    #content {
        padding-left: 0.5rem;
    }
}

.blocSelectionButton {
    height: 30px;
    width: 30px;
    border-radius: 15px;
    border-color: blue;
    color: white;
    background-color: blue;
    margin: 0.75rem 0;
    align-content: center;
    display: inline;
    vertical-align: middle;
    padding: 0rem 0 1rem 0;
}
</style>
