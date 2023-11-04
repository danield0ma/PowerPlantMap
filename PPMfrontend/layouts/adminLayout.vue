<template>
    <div>
        <AdminHeader id="head" class="scrollable-header" />
        <Nuxt id="body" />
    </div>
</template>

<script>
import AdminHeader from "../components/AdminHeader";
export default {
    name: "AdminLayout",

    components: {
        AdminHeader,
    },

    mounted() {
        let prevScrollPos = window.pageY;
        const header = this.$el.querySelector(".scrollable-header");

        window.addEventListener("scroll", () => {
            const currentScrollPos = window.pageY;
            if (prevScrollPos > currentScrollPos) {
                header.style.top = "0"; // Show the header when scrolling up
            } else {
                header.style.top = "-100px"; // Hide the header when scrolling down
            }
            prevScrollPos = currentScrollPos;
        });
    },
};
</script>

<style>
#head {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    background: rgba(255, 255, 255, 0.75);
    border: 0;
    z-index: 2;
    height: 3.5rem;
}

#body {
    position: relative;
    size: cover;
    height: 100vh;
    width: 100vw;
}

.scrollable-header {
    position: fixed; /* Fix the header at the top of the viewport */
    top: 0;
    left: 0;
    width: 100%;
    background-color: #333; /* Set a background color */
    z-index: 100; /* Ensure it's above other content */
    /* Add other styling as needed */
}
</style>
