<template>
	<div class="outerDiv">
		<div id="innerDiv">
			<NuxtLink to="/admin">
				<img v-cloak src="/electricity.png" width="30px" />
			</NuxtLink>
			<h4 style="margin: 0 0 0.1rem 0.75rem">
				<NuxtLink to="/admin">PowerPlantMap Admin</NuxtLink>
			</h4>
		</div>
		<ul>
			<p><NuxtLink to="/admin/powerplantseditor">Erőművek</NuxtLink></p>
			<p><NuxtLink to="/admin/email">E-mail lista</NuxtLink></p>
			<p><NuxtLink to="/admin/logout">Kijelentkezés</NuxtLink></p>
		</ul>
	</div>
</template>

<script>
export default {
	name: "AdminHeader",
};
</script>

<style>
body {
	background-color: gray;
}

.outerDiv {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin: 0;
	padding: 0 2rem;
	background-color: gray;
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
	color: blueviolet;
}

a:hover {
	text-decoration: none;
	color: darkgreen;
}

[v-cloak] {
	display: none;
}
</style>
