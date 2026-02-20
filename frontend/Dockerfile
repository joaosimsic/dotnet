FROM oven/bun:1 AS development
WORKDIR /app
COPY package.json bun.lockb* ./
RUN bun install
EXPOSE 5173
CMD ["bun", "run", "dev", "--host"]

FROM development AS build
COPY . .
RUN bun run build

FROM oven/bun:1-alpine AS production
WORKDIR /app
COPY --from=build /app/dist ./dist
COPY --from=build /app/package.json ./
RUN bun add serve
EXPOSE 3000
CMD ["bun", "run", "serve", "-s", "dist", "-l", "3000"]
