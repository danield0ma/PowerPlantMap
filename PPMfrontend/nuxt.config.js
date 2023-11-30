import path from "path";
import fs from "fs";
require("dotenv").config();
import "dotenv/config";

export default {
    server: {
        dev: process.env.NODE_ENV === "development",
        https:
            process.env.NODE_ENV === "production" &&
            process.env.GITHUB_ACTIONS !== "true"
                ? {
                      key: fs.readFileSync(
                          path.resolve(
                              "/etc/letsencrypt/live/powerplantmap.tech",
                              "privkey.pem"
                          )
                      ),
                      cert: fs.readFileSync(
                          path.resolve(
                              "/etc/letsencrypt/live/powerplantmap.tech",
                              "fullchain.pem"
                          )
                      ),
                  }
                : null,
    },

    head: {
        title: "PowerPlantMap",
        htmlAttrs: {
            lang: "en",
        },
        meta: [
            { charset: "utf-8" },
            {
                name: "viewport",
                content: "width=device-width, initial-scale=1",
            },
            { hid: "description", name: "description", content: "" },
            { name: "format-detection", content: "telephone=no" },
        ],
        link: [{ rel: "icon", type: "image/x-icon", href: "/electricity.ico" }],
    },

    css: ["mapbox-gl/dist/mapbox-gl.css", "@/assets/global.css"],

    plugins: [
        { src: "~/plugins/chart/chart.js", mode: "client", swcMinify: false },
    ],

    components: true,

    buildModules: ["@nuxtjs/fontawesome", "@nuxtjs/pwa"],

    modules: ["bootstrap-vue/nuxt", "@nuxtjs/auth", "@nuxtjs/axios", '@nuxtjs/dotenv'],

    axios: {
        baseURL: "https://powerplantmap.tech:5001",
    },

    auth: {
        strategies: {
            local: {
                scheme: "local",
                token: {
                    property: "token",
                },
                user: {
                    property: false,
                },
                endpoints: {
                    login: { url: "/api/Account/Login", method: "post" },
                    logout: { url: "/api/Account/Logout", method: "post" },
                    user: false, // {                        url: "/api/Account/GetCurrentUserProfile",                        method: "get",                    },
                },
            },
        },
    },

    fontawesome: {
        icons: {
            solid: true,
            brands: true,
        },
    },

    build: {
        babel: {
            compact: true,
        },
    },
};
