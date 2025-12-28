# ============================================================
# BARBERCLOUD — SISTEMA DE BARBEARIA (FULL STACK)
# ============================================================
# Plataforma completa de agendamento e gestão de barbearia
# com autenticação segura, controle de cargos e arquitetura
# preparada para produção.
#
# Este repositório documenta separadamente:
# - API (Back-end + Banco + Infraestrutura)
# - Front-end (App Web / PWA + Infraestrutura) em [FrontRepo](https://github.com/alonesch/barbearia-portifolio)
# ============================================================
# ============================================================
# BACK-END — API BARBERCLOUD
# ============================================================
# API REST responsável por autenticação, regras de negócio,
# agendamentos, disponibilidade, histórico e controle de usuários.
# ============================================================
# ----------------------------
# STACK BACK-END
# ----------------------------
# - .NET 9
# - ASP.NET Core Web API
# - Entity Framework Core
# - PostgreSQL
# - JWT + Refresh Token
# - Confirmação de e-mail
# - Docker + Docker Compose
# - Nginx (Reverse Proxy)
# ----------------------------
# ARQUITETURA
# ----------------------------
# - Arquitetura modular por domínio
# - Separação clara entre:
#   - Identidade (Usuario)
#   - Domínio (Cliente, Barbeiro)
# - Controllers versionados (v1 / v2)
# - DTOs para isolamento de entidades
# - Services com regras de negócio
# - Repositories para acesso a dados
# ----------------------------
# AUTENTICAÇÃO E SEGURANÇA
# ----------------------------
# - Login via email ou username
# - JWT assinado com claims controladas
# - Refresh Token persistido no banco
# - Confirmação de e-mail obrigatória
# - Bloqueio de login (403) para e-mails não confirmados
# - Controle de acesso por cargo:
#   - Admin
#   - Barbeiro
#   - Cliente
# ----------------------------
# BANCO DE DADOS
# ----------------------------
# Banco: PostgreSQL
#
# Entidades principais:
# - Usuario
# - Cliente
# - Barbeiro
# - Servico
# - Disponibilidade
# - Agendamento
#
# Relacionamentos:
# - Usuario 1:1 Cliente
# - Usuario 1:1 Barbeiro
# - Barbeiro 1:N Disponibilidade
# - Cliente 1:N Agendamento
# ----------------------------
# INFRAESTRUTURA (BACK-END)
# ----------------------------
# - API containerizada com Docker
# - Banco PostgreSQL em container dedicado
# - Docker Compose orquestrando serviços
# - Volumes persistentes para dados do banco
# - Variáveis sensíveis via .env
# - Reverse proxy com Nginx
# - SSL com Let's Encrypt
# - Ambientes separados (dev / prod)
# - CORS restritivo por domínio
# ----------------------------
# VERSIONAMENTO DA API
# ----------------------------
# Padrão de rotas:
# /api/v1/*
# /api/v2/*
#
# Exemplo:
# GET    /api/v2/cliente/me
# POST   /api/v2/auth/login
# PATCH  /api/v2/agendamento/status/{id}


