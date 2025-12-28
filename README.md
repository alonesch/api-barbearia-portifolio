<div align="center">

# BarberCloud.API

### Back-end e Infraestrutura

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)](https://jwt.io/)

</div>

---

## Sobre o Projeto

BarberCloud.API é a API central do sistema BarberCloud, responsável pela autenticação, execução das regras de negócio, persistência de dados e garantia de segurança da plataforma de agendamento da barbearia.

O projeto foi desenvolvido em **.NET 9** com foco em **arquitetura limpa**, separação clara de responsabilidades e preparação para ambientes de produção, seguindo práticas consolidadas de mercado em autenticação, versionamento e DevOps.

A API atua como o núcleo do sistema. Todas as validações críticas, permissões, regras de negócio e garantias de integridade de dados são executadas exclusivamente no backend. O front-end funciona apenas como consumidor da API, sem qualquer responsabilidade sensível.

---

## Arquitetura

A arquitetura é modular e orientada a domínio, separando explicitamente identidade e entidades de negócio. Essa abordagem reduz acoplamento, melhora a legibilidade do código e facilita a evolução do sistema.

O módulo de identidade concentra autenticação, tokens, claims e controle de acesso. O domínio representa o funcionamento real da barbearia, incluindo clientes, barbeiros, serviços, disponibilidade e agendamentos.

A organização interna segue responsabilidades bem definidas, com **controllers** responsáveis pela exposição dos endpoints, **services** pela execução das regras de negócio, **repositories** pelo acesso a dados e **DTOs** atuando como camada de isolamento entre domínio e transporte.

---

## Autenticação e Segurança

O sistema de autenticação utiliza **JWT (access token)** em conjunto com **refresh token** persistido em banco de dados, permitindo sessões seguras, renovação controlada e invalidação quando necessário.

As claims são sempre geradas no servidor, a partir do estado real do banco de dados, impossibilitando qualquer tipo de manipulação por parte do cliente.

A confirmação de e-mail é obrigatória para liberação do login. Usuários não confirmados recebem bloqueio explícito, aumentando a segurança e a confiabilidade dos cadastros.

O controle de acesso é baseado em cargos bem definidos: **administrador**, **barbeiro** e **cliente**. Cada rota valida permissões diretamente no backend, garantindo que nenhuma regra sensível dependa de dados vindos do front-end.

---

## Banco de Dados

O banco de dados utiliza **PostgreSQL**, com modelagem orientada ao domínio do sistema e foco em integridade, consistência e clareza de relacionamentos.

As principais entidades incluem usuário, cliente, barbeiro, serviço, disponibilidade, agendamento e refresh token. Os relacionamentos refletem fielmente o fluxo real da aplicação, facilitando manutenção e evolução futura.

---

## Agendamentos e Regras de Negócio

Todo o controle de agendamentos é centralizado na API. A validação de status, conflitos de horário, disponibilidade de barbeiros e histórico de atendimentos ocorre exclusivamente no backend, garantindo consistência mesmo em cenários concorrentes.

---

## Versionamento

A API adota versionamento explícito de rotas, permitindo evolução segura sem quebra de contratos existentes e garantindo estabilidade para clientes já em produção.

---

## Infraestrutura

A infraestrutura é baseada em **Docker**, com a API executando em container isolado e o banco PostgreSQL em container dedicado. O ambiente é orquestrado via **Docker Compose**, com volumes persistentes e variáveis sensíveis configuradas exclusivamente por ambiente.

Um **reverse proxy com Nginx** é utilizado para gerenciamento de tráfego, aplicação de HTTPS com certificados válidos e controle de CORS por domínio. Os ambientes de desenvolvimento e produção são isolados, cada um com suas próprias configurações e políticas de acesso.

---

## Estado Atual

O projeto encontra-se em **estado estável de produção**, com autenticação completa, banco estruturado, fluxo real de agendamentos funcional e base arquitetural preparada para crescimento, novas funcionalidades e escalabilidade como produto.

---

## Autor

**Cristian Schmidt**  
Desenvolvedor Full Stack  
Stack principal: C#, .NET, PostgreSQL, Docker, DevOps

---

<div align="center">

**BarberCloud.API** • Desenvolvido com .NET 9

</div>