<template>
	<div>
		<header>
			<AdminHeader id="head" />
		</header>
		<slot id="body" />
	</div>
</template>

<script>
import AdminHeader from "../components/Headers/AdminHeader";
export default {
	name: "admin-layout",

	components: {
		AdminHeader,
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
</style>
