FROM node:20-alpine AS dev
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .

ARG PUBLIC_API_BASE_URL
ENV PUBLIC_API_BASE_URL=$PUBLIC_API_BASE_URL
ENV HOST=0.0.0.0
EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]


# Stage 1: Build
FROM node:20-alpine AS build-stage
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .

ARG PUBLIC_API_BASE_URL
ENV PUBLIC_API_BASE_URL=$PUBLIC_API_BASE_URL

RUN npm run build

# Stage 2: Serve
FROM nginx:stable-alpine AS production-stage

# Ensure the destination is clean
RUN rm -rf /usr/share/nginx/html/*

# Copy the CONTENTS of build into html
COPY --from=build-stage /app/build/. /usr/share/nginx/html/

# Copy your config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Set permissions so the 'nginx' user can actually read the files
RUN chmod -R 755 /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]