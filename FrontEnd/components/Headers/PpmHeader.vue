<template>
	<div class="outerDiv">
		<div id="innerDiv">
			<NuxtLink to="/">
				<img src="/electricity.png" width="30px" />
			</NuxtLink>
			<h4 style="margin: 0 0 0.1rem 0.75rem">
				<nuxt-link to="/">PowerPlantMap</nuxt-link>
			</h4>
		</div>
		<ul>
			<p><NuxtLink to="/">Map</NuxtLink></p>
			<p><NuxtLink to="/admin">Admin</NuxtLink></p>
			<p><NuxtLink to="/about">About</NuxtLink></p>
		</ul>
	</div>
</template>

<script>
export default {
	name: "PpmHeader",
};
</script>

<style>
.outerDiv {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin: 0;
	padding: 0 2rem;
	/* background: rgba(255, 255, 255, 0.75); */
}

#innerDiv {
	display: flex;
}

ul {
	display: flex;
	margin: 0;
	text-decoration: none;
	text-transform: none;
}

ul p {
	padding: 1rem;
	text-decoration: none;
	margin: 0;
	color: blue;
}

a:hover {
	text-decoration: none;
	color: darkblue;
}
</style>
