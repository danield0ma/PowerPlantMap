<template>
	<div>
		<header>
			<PpmHeader id="head" />
		</header>
		<slot id="body" />
	</div>
</template>

<script>
import PpmHeader from "../components/Headers/PpmHeader";
export default {
	name: "default",

	components: {
		PpmHeader,
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
