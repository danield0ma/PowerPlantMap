<template>
	<NuxtError :error="{ statusCode: 404, message: 'Page not found' }">
		<template #default="{ error }">
			<div>
				<h1>{{ error.message }}</h1>
				<p>The page you are looking for could not be found.</p>
			</div>
		</template>
	</NuxtError>
</template>

<script>
import { NuxtError } from "#app";

export default {
	name: "NotFound",
	components: {
		NuxtError,
	},
};
</script>

<style scoped>
h1 {
	color: red;
}
</style>
