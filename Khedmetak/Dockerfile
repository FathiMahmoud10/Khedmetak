# ── Build Stage ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & restore
COPY . .
RUN dotnet restore

# Build & publish
RUN dotnet publish -c Release -o /app/publish

# ── Runtime Stage ─────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Khedmetak.dll"]
