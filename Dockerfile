# ============================
# 🟩 FASE DE BUILD
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Cache eficiente das dependências do projeto
COPY BarbeariaPortifolio.API.csproj ./

# Restaura dependências
RUN dotnet restore BarbeariaPortifolio.API.csproj

# Copia toda a solução
COPY . .

# Publica em modo Release
RUN dotnet publish BarbeariaPortifolio.API.csproj -c Release -o /app/publish /p:UseAppHost=false


# ============================
# 🟦 FASE FINAL (RUNTIME)
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copia somente o resultado final
COPY --from=build /app/publish .

# timezone BR
ENV TZ=America/Sao_Paulo

# Informando a porta para o Render
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "BarbeariaPortifolio.API.dll"]
