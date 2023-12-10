export default function ({ app, redirect }) {
    if (!app.$auth.loggedIn) {
        return redirect("/login");
    }

    if (app.$auth.user.role !== "admin") {
        return redirect("/admin");
    }
}
