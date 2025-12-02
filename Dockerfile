# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas o csproj primeiro (para melhor cache)
COPY BarbeariaPortfolio.API.csproj ./

# Restaura dependências
RUN dotnet restore BarbeariaPortfolio.API.csproj

# Copia todo o restante do projeto
COPY . .

# Publica a aplicação
RUN dotnet publish BarbeariaPortfolio.API.csproj -c Release -o /app/publish


# ====== FASE FINAL (Runtime) ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BarbeariaPortfolio.API.dll"]
