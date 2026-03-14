FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Sysacad.Server.csproj", "."]
RUN dotnet restore "./Sysacad.Server.csproj"
COPY . .
RUN dotnet build "Sysacad.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sysacad.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sysacad.Server.dll"]