# 💈 BarberCloud.API — Back-end e Infraestrutura
#
# BarberCloud.API é a API oficial do sistema BarberCloud, responsável por
# autenticação, regras de negócio, persistência de dados e segurança da
# plataforma de agendamento da barbearia.
#
# O projeto foi desenvolvido em .NET 9 com foco em arquitetura limpa,
# separação de responsabilidades e preparação para ambientes de produção,
# adotando práticas reais de mercado em autenticação, versionamento e DevOps.
#
# A API atua como núcleo do sistema, sendo a única responsável por validar
# permissões, executar regras de negócio e garantir a integridade dos dados.
#
# Toda a comunicação com o front-end ocorre por meio de endpoints REST
# versionados, protegidos por autenticação JWT e controle de acesso por cargo.
#
# A arquitetura do projeto é modular e orientada a domínio, separando de forma
# explícita o conceito de identidade (Usuário) das entidades de negócio
# (Cliente, Barbeiro, Agendamento, etc.). Essa decisão garante clareza
# arquitetural, evita acoplamento indevido e facilita a evolução do sistema.
#
# O fluxo de autenticação utiliza JWT (Access Token) combinado com Refresh
# Token persistido em banco, permitindo sessões seguras, renovação controlada
# de tokens e invalidação quando necessário. As claims são sempre geradas no
# servidor, a partir do estado real do banco, impossibilitando qualquer
# manipulação por parte do front-end.
#
# O sistema exige confirmação de e-mail para liberação do login. Usuários não
# confirmados recebem bloqueio explícito, garantindo maior segurança e
# confiabilidade dos cadastros.
#
# O controle de acesso é baseado em cargos bem definidos:
# Admin, Barbeiro e Cliente. Cada rota da API valida permissões diretamente no
# backend, assegurando que nenhuma regra sensível dependa do cliente.
#
# O banco de dados utiliza PostgreSQL, com modelagem orientada ao domínio do
# sistema. As entidades principais incluem Usuário, Cliente, Barbeiro,
# Serviço, Disponibilidade, Agendamento e RefreshToken, com relacionamentos
# bem definidos e coerentes com o fluxo real da aplicação.
#
# O controle de agendamentos é totalmente centralizado na API. A lógica de
# status, conflitos de horário, disponibilidade de barbeiros e histórico de
# atendimentos é validada exclusivamente no backend, garantindo consistência
# mesmo em cenários concorrentes.
#
# A API adota versionamento explícito de rotas, permitindo evolução segura sem
# quebra de contratos existentes. Novas versões podem ser introduzidas sem
# impacto direto sobre clientes já em produção.
#
# A infraestrutura do projeto é baseada em Docker, com a API executando em
# container isolado e o banco PostgreSQL em container dedicado. O ambiente é
# orquestrado via Docker Compose, com volumes persistentes para dados e
# variáveis sensíveis configuradas exclusivamente via ambiente.
#
# Um reverse proxy com Nginx é utilizado para gerenciamento de tráfego,
# aplicação de HTTPS com certificados válidos e controle de CORS por domínio.
# Os ambientes de desenvolvimento e produção são isolados, cada um com suas
# próprias configurações, chaves e políticas de acesso.
#
# O projeto encontra-se em estado estável de produção, com autenticação
# completa, banco estruturado, fluxo real de agendamentos funcional e base
# arquitetural preparada para crescimento, novas features e escalabilidade
# como produto SaaS.
#
# Autor: Cristian Schmidt
# Desenvolvedor Full Stack
# Stack principal: C#, .NET, PostgreSQL, Docker, DevOps
