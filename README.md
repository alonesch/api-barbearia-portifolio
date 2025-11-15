#  API — Barbearia Portifolio  
Back-end desenvolvido em .NET 8 + MySQL para o sistema de agendamento da barbearia.

##  Visão Geral
Esta API foi construída para gerenciar todo o fluxo de agendamentos de uma barbearia, incluindo clientes, barbeiros, serviços, autenticação e painel administrativo.  
Ela funciona como base de dados e regras de negócio para o front-end hospedado na Vercel.

A API está publicada em produção no Railway.

---

##  Arquitetura
O projeto segue o padrão de **3 camadas**:

- **Controllers**: recebem a requisição e retornam resposta padronizada.  
- **Services**: camada onde toda a lógica de negócio acontece.  
- **Repositories**: camada responsável pela comunicação com o banco via Entity Framework Core.  
- **DataContext**: mapeamento das entidades e relacionamentos.

---

##  Autenticação e Segurança
O sistema utiliza:

- **JWT** (Access + Refresh token)
- **Hash de Refresh Token**
- **Roles (Administrador / Barbeiro)**
- **Rate Limiting** para rotas sensíveis
- **Chave JWT por variável de ambiente**
- **CORS configurado para produção**

A sessão do administrador é controlada pelo front e validada pela API.

---

##  Banco de Dados
Tecnologia: **MySQL**

### Principais tabelas:
- Cliente  
- Barbeiro  
- Serviço  
- Agendamento  
- AgendamentoServico  
- Usuario  
- RefreshToken  

### Status numéricos de agendamento:
1 pendente
2 confirmado
3 aguardando pagamento
4 pago
5 cancelado pelo cliente
6 cancelado pelo barbeiro
7 finalizado
8 extra
9 extra


---

##  Funcionalidades Implementadas

### ✔ Criação de Agendamento
- Cria cliente automaticamente se telefone não existir  
- Reaproveita cliente já existente  
- Valida conflito de horário  
- Salva serviços vinculados  
- Aceita observação opcional  

### ✔ Listagem Completa (Admin)
Retorna:
- cliente  
- barbeiro  
- serviços  
- observação  
- status (com texto)  
- data e hora  

### ✔ Alteração de Status
Fluxo completo implementado:  
`PATCH /agendamentos/{id}/status`

### ✔ Autenticação JWT
- Login  
- Refresh token  
- Renovação segura  

---

##  Deploy
- API hospedada no **Railway**  
- Rodando em **Production**  
- Banco em MySQL remoto  

---

##  Status Atual do Projeto
A API está **completamente funcional**, estável e integrada ao front.  
O último recurso desenvolvido foi o fluxo de **alteração de status**, 100% funcional em produção.
