# ==== BASE STAGE ====
FROM node:16-alpine AS base
WORKDIR /app
COPY package*.json ./
RUN npm ci

# ==== BUILD STAGE ====
FROM base AS build
COPY . .
RUN npm run build

# ==== PRODUCTION STAGE ====
FROM node:16-alpine AS production
WORKDIR /app

# Install `serve` for static file hosting (optional)
RUN npm install -g serve

# Copy only the built output from the previous stage
COPY --from=build /app/dist ./dist

# Expose the port (default for `serve`)
EXPOSE 3000

CMD ["serve", "-s", "dist"]