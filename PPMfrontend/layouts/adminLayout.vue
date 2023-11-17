<template>
    <div>
        <AdminHeader id="nuxtHead" />
        <Nuxt id="nuxtBody" />
    </div>
</template>

<script>
import AdminHeader from "../components/Headers/AdminHeader";
export default {
    name: "AdminLayout",

    components: {
        AdminHeader,
    },
};
</script>

<style scoped>
#nuxtHead {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    background: rgba(255, 255, 255, 0.75);
    border: 0;
    z-index: 2;
    height: 3.5rem;
}

#nuxtBody {
    position: relative;
    size: cover;
    height: 100vh;
    width: 100vw;
}
</style>
