export default function ({ app, redirect }) {
    if (!app.$auth.loggedIn) {
        return redirect("/login");
    }

    if (!["admin", "user"].includes(app.$auth.user.role)) {
        return redirect("/admin");
    }
}
