
# 💈 BarbeariaPortfolio.API — Back-end em .NET 9

API oficial do sistema de agendamentos da barbearia, desenvolvida em **.NET 9**, com arquitetura limpa, autenticação JWT e deploy automatizado no Render.  
Ela fornece toda a base de dados, regras de negócio e segurança utilizadas pelo front-end hospedado na Vercel.

---

##  Visão Geral

Esta API é responsável por todo o fluxo operacional do sistema da barbearia:

- Gestão de clientes  
- Agenda dos barbeiros  
- Cadastro de serviços  
- Agendamentos com múltiplos serviços  
- Autenticação com JWT  
- Painel administrativo integrado ao front-end  

A aplicação roda em ambientes **DEV** e **PRODUÇÃO**, ambos na plataforma **Render**, utilizando **PostgreSQL** como banco de dados.

---

##  Arquitetura

A API segue o padrão de **3 camadas**, garantindo organização, fácil manutenção e escalabilidade:

- **Controllers** — Entrada da requisição e saída da resposta  
- **Services** — Regras de negócio  
- **Repositories** — Persistência com Entity Framework Core  
- **DataContext** — Mapeamento de entidades e migrations  

A arquitetura foi planejada para suportar crescimento, testes e integrações futuras.

---

##  Autenticação e Segurança

A API utiliza um sistema robusto de segurança com:

- **JWT (Access Token + Refresh Token)**  
- **Refresh Token criptografado**  
- **Issuer, Audience e Key configurados via appsettings + variáveis de ambiente**  
- **Middleware global para padronizar erros**  
- **CORS separado por ambiente (DEV/PRD)**  
- **Usuário restrito PostgreSQL (`barber_api_user`) com permissões controladas**  

---

##  Banco de Dados — PostgreSQL

Tecnologia atual: **PostgreSQL (Render)**  
Ambientes separados:

- **dev_barber_db**  
- **prd_barber_db**

Roles configuradas:

- `barber_api_user` → apenas SELECT, INSERT, UPDATE, DELETE  
- Permissões automáticas para novas tabelas (default privileges)  

### Principais tabelas

- Cliente  
- Barbeiro  
- Serviço  
- Agendamento  
- AgendamentoServico  
- Usuario  
- RefreshToken  

### Status do Agendamento

| Código | Status                   |
|--------|--------------------------|
| 1      | Pendente                 |
| 2      | Confirmado               |
| 3      | Aguardando pagamento     |
| 4      | Pago                     |
| 5      | Cancelado pelo cliente   |
| 6      | Cancelado pelo barbeiro  |
| 7      | Finalizado               |
| 8      | Extra                    |
| 9      | Extra                    |

---

##  Funcionalidades Implementadas

###  Criação de Agendamento
- Geração automática de cliente quando telefone não existe  
- Reuso de cliente existente  
- Validação de conflitos de horário  
- Suporte a múltiplos serviços por agendamento  
- Observação opcional  

###  Listagem Administrativa
Retorno completo contendo:

- Cliente  
- Barbeiro  
- Serviços  
- Status + descrição  
- Observação  
- Data e hora  

###  Alteração de Status  
Endpoint:  
`PATCH /agendamentos/{id}/status`

###  Autenticação JWT
- Login seguro  
- Renovação via Refresh Token  
- Proteção de rotas por Role  

---

##  DevOps — Infraestrutura Moderna

###  Dockerfile Multi-Stage
- Build otimizado em .NET SDK  
- Runtime enxuto em ASP.NET 9  
- Timezone configurado  
- Exposto em `0.0.0.0:8080` (Render)

###  .dockerignore
- Reduz contexto de build  
- Remove binários, obj, cache, node_modules etc.

###  Migrations Automáticas
- Executadas no boot da aplicação  
- Tratamento seguro de erros  

### Health Check
Endpoint para monitoramento no Render:
GET /ping → "pong"


###  CI/CD com Render
- Deploy automático via Dockerfile  
- Ambientes DEV e PRD separados  
- Health Check obrigatório  
- Variáveis de ambiente seguras  

---

##  Deploy

**Back-end:**  
- Deploy usando Docker + Render  
- PostgreSQL dedicado (DEV/PRD)  
- Permissões refinadas no banco  
- Logs, CORS e JWT configurados por ambiente  

**Front-end:**  
- Hospedado na Vercel  
- Totalmente integrado com esta API  

---

##  Status Atual do Projeto

A API está em **estado estável**, com:

- Infraestrutura DevOps completa  
- Segurança reforçada (JWT + Roles + CORS)  
- Banco PostgreSQL estruturado  
- Fluxo de agendamentos 100% funcional  
- Painel administrativo integrado  
- Deploy contínuo totalmente operacional  

---

