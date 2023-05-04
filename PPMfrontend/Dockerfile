# Dockerfile
FROM node:16.20.0-alpine

# create destination directory
RUN mkdir -p /usr/src/nuxt-app
WORKDIR /usr/src/nuxt-app
COPY letsencrypt /etc/letsencrypt

# update and install dependency
RUN apk update && apk upgrade

# copy the app, note .dockerignore
COPY . /usr/src/nuxt-app/
RUN npm install
RUN npm run build

EXPOSE 3001

ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3001

ENTRYPOINT [ "npm", "start" ]
