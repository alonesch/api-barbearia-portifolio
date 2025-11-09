# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "BarbeariaPortifolio.API.csproj"
RUN dotnet publish "BarbeariaPortifolio.API.csproj" -c Release -o /app/publish

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BarbeariaPortifolio.API.dll"]
