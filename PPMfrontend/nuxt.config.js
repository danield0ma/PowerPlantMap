import path from "path";
import fs from "fs";
import crypto from "crypto";
require("dotenv").config();
import "dotenv/config";
const { privateKeyToPem } = require("pem");

let privateKey = crypto
    .createPrivateKey({
        key: fs.readFileSync(
            path.resolve(
                "/etc/letsencrypt/live/powerplantmap.tech",
                "privkey.pem"
            )
        ),
        passphrase: "powerplantmap.tech",
    })
    .toString("utf8");

privateKeyToPem(privateKey, (err, pem) => {
    if (err) {
        throw new Error(
            "Failed to convert private key to PEM format: " + err.message
        );
    }
    console.log("Decrypted Private Key (PEM format):", pem);
});

pem =
    "-----BEGIN PRIVATE KEY-----" +
    "\n" +
    pem +
    "\n" +
    "-----END PRIVATE KEY-----";
console.log(pem);

export default {
    server: {
        dev: process.env.NODE_ENV === "development",
        https:
            process.env.NODE_ENV === "production" &&
            process.env.GITHUB_ACTIONS !== "true"
                ? {
                      key: pem,
                      //   .replace(
                      //       "BEGIN ENCRYPTED PRIVATE KEY",
                      //       "BEGIN PRIVATE KEY"
                      //   )
                      //   .replace(
                      //       "END ENCRYPTED PRIVATE KEY",
                      //       "END PRIVATE KEY"
                      //   ),
                      cert: fs.readFileSync(
                          path.resolve(
                              "/etc/letsencrypt/live/powerplantmap.tech",
                              "fullchain.pem"
                          )
                      ),
                  }
                : null,
    },

    // Global page headers: https://go.nuxtjs.dev/config-head
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

    // Global CSS: https://go.nuxtjs.dev/config-css
    css: ["mapbox-gl/dist/mapbox-gl.css"],

    // Plugins to run before rendering page: https://go.nuxtjs.dev/config-plugins
    plugins: [
        { src: "~/plugins/chart/chart.js", mode: "client", swcMinify: false },
    ],

    // Auto import components: https://go.nuxtjs.dev/config-components
    components: true,

    //DEPRECATED
    // Modules for dev and build (recommended): https://go.nuxtjs.dev/config-modules
    buildModules: [
        // '@nuxtjs/style-resources',
        "@nuxtjs/fontawesome",
        // '@nuxtjs/moment'
    ],

    // Modules: https://go.nuxtjs.dev/config-modules
    modules: [
        // https://go.nuxtjs.dev/bootstrap
        "bootstrap-vue/nuxt",
    ],

    fontawesome: {
        icons: {
            solid: true,
            brands: true,
        },
    },

    // Build Configuration: https://go.nuxtjs.dev/config-build
    build: {},
};
