# Test image for Prowlarr with Anidub/Toloka/RuTracker title-parser fixes.
# Build stages run natively (BUILDPLATFORM), backend cross-publishes to linux-x64.
FROM --platform=$BUILDPLATFORM node:20-bookworm AS frontend
WORKDIR /build
COPY package.json yarn.lock tsconfig.json ./
COPY frontend ./frontend
RUN yarn install --frozen-lockfile --network-timeout 120000
RUN yarn run build --env production

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /build
# Root .editorconfig carries the StyleCop severity overrides; without it the build fails on SA rules.
# Logo/ is embedded as a resource by NzbDrone.Core.
COPY .editorconfig ./
COPY Logo ./Logo
COPY src ./src
RUN --mount=type=cache,target=/root/.nuget \
    dotnet publish src/NzbDrone.Console/Prowlarr.Console.csproj \
      -c Release -f net8.0 -r linux-x64 --self-contained \
      -p:SolutionDir=/build/src/ \
      -p:Platform=Posix \
      -o /build/publish \
    # Prowlarr.Mono is loaded dynamically by name on POSIX, publish it alongside
    && dotnet publish src/NzbDrone.Mono/Prowlarr.Mono.csproj \
      -c Release -f net8.0 -r linux-x64 --self-contained \
      -p:SolutionDir=/build/src/ \
      -p:Platform=Posix \
      -o /build/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-bookworm-slim
RUN apt-get update \
    && apt-get install -y --no-install-recommends libsqlite3-0 libicu72 curl ca-certificates \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=backend /build/publish ./bin
COPY --from=frontend /build/_output/UI ./bin/UI
EXPOSE 9696
VOLUME /config
ENTRYPOINT ["/app/bin/Prowlarr", "-nobrowser", "-data=/config"]
