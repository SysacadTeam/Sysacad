# ── Stage 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore — copy only .csproj files first for layer caching
COPY ["Sysacad.Server/Sysacad.Server.csproj",  "Sysacad.Server/"]
COPY ["Sysacad.Client/Sysacad.Client.csproj",   "Sysacad.Client/"]
COPY ["Sysacad.Shared/Sysacad.Shared.csproj",   "Sysacad.Shared/"]
RUN dotnet restore "Sysacad.Server/Sysacad.Server.csproj"

# Copy all sources and publish
COPY . .
RUN dotnet publish "Sysacad.Server/Sysacad.Server.csproj" \
        -c Release \
        -o /app/publish \
        /p:UseAppHost=false

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sysacad.Server.dll"]
