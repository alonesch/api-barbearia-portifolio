# API Barbercloud

API REST construída em .NET 9 para gerenciar uma barbearia/portfólio — autenticação por JWT, segurança de senhas com BCrypt e banco de dados PostgreSQL. Projetada para ser executada em VPS (via Docker ou diretamente com Kestrel + Nginx). Fornece endpoints para autenticação, usuários, serviços, agendamentos e gerenciamento de portfólio.

[![.NET](https://img.shields.io/badge/.NET-9-blue)]()
[![License](https://img.shields.io/badge/license-MIT-green)]()
[![Status](https://img.shields.io/badge/status-production-ready-brightgreen)]()



---

Sumário
- Sobre
- Tecnologias
- Estrutura do repositório
- Diagramas simples (fluxos)
- Configuração (appsettings / variáveis)
- Como executar (local / Docker)
- Migrations (EF Core)
- Endpoints principais
- Segurança
- Troubleshooting rápido
- Contribuição
- Licença

---

Sobre
Este repositório contém a API que alimenta o portfólio da barbearia. A intenção foi fazer algo simples, seguro e fácil de publicar em um VPS com Docker. O README aqui foi pensado para facilitar quem está começando (eu mesmo usei isso quando comecei) — objetivo: claro, direto e útil.

Tecnologias
- .NET 9 (C#)
- ASP.NET Core Web API
- Entity Framework Core (migrations)
- PostgreSQL
- BCrypt (hash de senhas)
- JWT (tokens)
- Docker + docker-compose

Estrutura do repositório (resumida)
- .dockerignore
- .github/
- .gitignore
- BarbeariaPortifolio.API.csproj
- BarbeariaPortifolio.API.sln
- Dockerfile
- docker-compose.yml
- Program.cs
- appsettings.json
- Migrations/
- Modules/ (features)
- Shared/ (DTOs, helpers)
- Templates/
- Properties/

Se algo estiver em outro lugar, ajuste conforme seu fluxo — aqui eu listei o que é mais comum no projeto.

---

Diagramas simples e explicativos
Obs: removi os fluxogramas complexos e usei diagramas ASCII bem simples para facilitar leitura e garantir que tudo renderize sem erros.

1) Fluxo de autenticação (resumo)
Cliente -> API (POST /api/auth/login)
  -> API busca usuário no repositório (UserRepository / EF Core)
    -> Banco Postgres retorna usuário com PasswordHash
  -> API verifica senha com BCrypt
    -> Se ok: API gera JWT e retorna token ao cliente
    -> Se não: retorna 401 Unauthorized

Representação simples:
[Client] --POST /auth/login--> [API] --query user--> [Postgres]
[API] --BCrypt verify--> [PasswordHash]
[API] --generate JWT--> [Client] (token)

2) Pipeline de requisição (o que acontece em cada request)
[Client] -> [Ingress/LoadBalancer?] -> [Middleware: Logging] -> [Middleware: CORS] -> [Auth JWT] -> [Authorization] -> [Controller] -> [Service] -> [Repository/EF] -> [Postgres] -> [Response]

3) Deploy básico (passos)
1. dotnet publish -c Release
2. docker build (imagem multi-stage)
3. docker run / docker-compose up -d na VPS
4. container da API sobe + container do Postgres (ou DB externo)
5. aplicar migrations (automático na inicialização ou manual)

4) Modelo de dados (exemplo simplificado)
Usuários:
- Id (int, PK)
- Name (string)
- Email (string, único)
- PasswordHash (string)
- Role (string)
- CreatedAt, UpdatedAt

Serviços / Portfolio:
- Id (int, PK)
- Title (string)
- Description (string)
- Price (decimal)
- CreatedBy (FK -> Users.Id)
- CreatedAt, UpdatedAt

---

Configuração (appsettings.json exemplo)
Substitua valores sensíveis por variáveis de ambiente em produção.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=barbearia;Username=postgres;Password=secret"
  },
  "Jwt": {
    "Key": "COLOQUE_AQUI_UMA_CHAVE_SECRETA_MUITO_LONGA",
    "Issuer": "barbearia.api",
    "Audience": "barbearia.client",
    "ExpiresMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Variáveis de ambiente úteis para Docker/VPS
- POSTGRES_USER
- POSTGRES_PASSWORD
- POSTGRES_DB
- ASPNETCORE_ENVIRONMENT
- ConnectionStrings__DefaultConnection
- Jwt__Key, Jwt__Issuer, Jwt__Audience, Jwt__ExpiresMinutes

Dica: use env_file ou Docker secrets em produção; não comite secrets no repo.

---

Como executar localmente (modo simples)
1. Clone
   - git clone https://github.com/alonesch/api-barbearia-portifolio.git
2. Ajuste appsettings.json ou configure variáveis de ambiente
3. Restore e run
   - dotnet restore
   - dotnet ef database update
   - dotnet run
4. A API costuma rodar em https://localhost:5001 ou http://localhost:5000

Se preferir rodar com o Postgres via Docker local:
- docker run --name pg -e POSTGRES_PASSWORD=secret -e POSTGRES_DB=barbearia -p 5432:5432 -d postgres:15

---

Como executar com Docker / docker-compose
1. Build e subir:
   - docker-compose up --build -d
2. Logs:
   - docker-compose logs -f app
3. Parar:
   - docker-compose down

Se as migrations não rodarem automaticamente, abra o shell do container e rode:
- docker-compose exec app dotnet ef database update

---

Migrations (EF Core)
Comandos úteis (no diretório do .csproj):
- dotnet ef migrations add NomeDaMigration
- dotnet ef database update
- dotnet ef migrations remove

Se estiver usando múltiplos projetos, verifique os parâmetros --project e --startup-project.

---

Endpoints principais (modelo)
- POST /api/auth/register — registrar usuário
- POST /api/auth/login — autenticar e receber JWT
- GET /api/users/me — perfil do usuário (Bearer token)
- GET /api/services — listar serviços
- POST /api/services — criar serviço (autorizado)
- PUT /api/services/{id} — atualizar serviço (autorizado)
- DELETE /api/services/{id} — deletar serviço (autorizado)

Exemplo de login com curl:
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"email":"teste@ex.com","password":"senha"}'

Resposta esperada:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": { "id": 1, "email": "teste@ex.com", "name": "Fulano" }
}

---

Segurança (práticas que apliquei / recomendadas)
- Senhas: armazenadas como hash BCrypt (nunca em texto).
- JWT: chave secreta longa; validar assinatura e expiração no middleware.
- HTTPS: obrigatório em produção.
- CORS: configurar origens permitidas.
- Rate limiting / proteção de brute force: considerar se houver muitas tentativas de login.
- Secrets: usar secret manager (Vault, AWS Secrets Manager) em ambientes reais.

---

Troubleshooting rápido
- Erro de conexão com DB:
  - Verifique connection string, usuário/senha, se o container do Postgres está rodando e portas.
- Token JWT inválido:
  - Veja se a chave configurada no ambiente do container é a mesma que a aplicação usa para gerar tokens.
- Migrations não aplicam:
  - Verifique se o projeto correto foi informado ao dotnet ef; verifique logs de startup.

---

Contribuição
- Abra uma issue para bugs ou sugestões.
- Para PRs: mantenha o estilo C#, escreva commits claros e, se possível, inclua testes.
- Se quiser, posso preparar um PR com documentação ou pequenas correções — só dizer.

