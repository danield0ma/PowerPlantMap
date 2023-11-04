<template>
    <div class="modal-backdrop">
        <div class="modal-content">
            <h1>Új erőmű létrehozása</h1>
            <form action="">
                <label for="id"></label>
                <input id="id" type="text" placeholder="ASD" v-model="Id" />
                <label for="name"></label>
                <input
                    id="name"
                    type="text"
                    placeholder="asdfgh"
                    v-model="Name"
                />
                <label for="description"></label>
                <input
                    id="description"
                    type="text"
                    placeholder="leiras"
                    v-model="Description"
                />
                <button @click="$emit('close')">Close</button>
            </form>
        </div>
    </div>
</template>

<script>
export default {
    name: "AddPowerPlant",

    data() {
        return {
            Id: "",
            Name: "",
            Description: "",
            OperatorCompany: "",
            Webpage: "",
            Longitude: "",
            Latitude: "",
            Color: "",
            Address: "",
            IsCountry: "",
            Blocs: [],
        };
    },

    methods: {
        addPowerPlant() {
            console.log(this.Name, this.Description);
        },
    },
};
</script>

<style>
.modal-backdrop {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgb(0, 0, 0, 0.5);
    opacity: 0.5;
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 999;
}

.modal-content {
    background-color: #fff; /* Light background for better readability */
    padding: 20px;
    margin: 0 30vw;
    border-radius: 15px;
    border: 3px solid #333; /* Dark border for contrast */
    position: fixed; /* Center the modal */
    /* top: 50%;
    left: 50%; */
    transform: translate(-50%, -50%);
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); /* 3D effect */
    max-width: 90%; /* Prevent the modal from being too wide on small screens */
    z-index: 1000; /* Ensure the modal is above other elements */
}
</style>
