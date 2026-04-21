# FCG (Fiap Cloud Games) 

Bem-vindo ao repositório do **FCG**, uma API para gerenciamento de lojas de jogos desenvolvida em **.NET 10**. Este projeto utiliza os princípios de **Clean Architecture** (Arquitetura Limpa) e **Domain-Driven Design (DDD)** para criar um sistema escalável e de fácil manutenção.

## 🚀 Tecnologias Utilizadas

- **.NET 10** (Minimal APIs)
- **Entity Framework Core 10** (com Microsoft SQL Server)
- **Autenticação:** JWT Bearer (JSON Web Tokens)
- **Validação:** FluentValidation
- **Documentação de API:** Swagger (OpenAPI)
- **Arquitetura:** Clean Architecture / DDD (Domain, Application, Infrastructure, API)

## 📦 Estrutura do Projeto

O projeto está dividido nas seguintes camadas:

- **src/FCG.API:** Camada de apresentação contendo os Endpoints (Minimal APIs), Middlewares de tratamento de erro, e injeção de dependências.
- **src/FCG.Application:** Camada de aplicação contendo os Casos de Uso (Services), Interfaces (Contratos) e validações com FluentValidation.
- **src/FCG.Domain:** Núcleo do sistema, contendo as entidades de domínio (`User`, `Game`, `Promotion`, `Order`, `OrderItem`, `LibraryItem`) e regras de negócio essenciais.
- **src/FCG.Infrastructure:** Camada de acesso a dados (Entity Framework Core DbContext, Migrations) e implementação dos repositórios.
- **test/FCG.Test:** Projetos de testes (unitários/integração).

## ✨ Funcionalidades Principais e Atualizações Recentes

A API gerencia um catálogo de jogos e permite aos usuários comprar jogos que são então adicionados à sua biblioteca (`LibraryItem`). 

Recentemente ocorreram mudanças arquiteturais significativas:

1. **Fluxo de Compras Baseado em Pedidos (Order-Based Flow):** 
   - A aquisição de jogos foi refatorada de uma adição direta à biblioteca para um fluxo robusto de comércio eletrônico. 
   - O sistema agora suporta a criação de um pedido (`Order`) que pode conter múltiplos jogos (`OrderItem`), simulando um processo de aprovação de pagamento antes que os jogos sejam disponibilizados na biblioteca do usuário.
2. **Sistema de Promoções (Promotions):**
   - Implementação da entidade de Promoção (`Promotion`), permitindo que usuários com perfil de administrador criem ofertas promocionais para jogos específicos, definindo descontos e períodos de validade.
3. **Busca de Jogos Integrada às Promoções:**
   - A funcionalidade de busca de jogos foi atualizada. Agora, ao buscar pelo catálogo, a API automaticamente verifica e integra as informações de promoções ativas ao retorno, informando o status promocional e os preços atualizados para o cliente final.
4. **Migração de Banco de Dados:**
   - A configuração de banco de dados e persistência foi padronizada e migrada com sucesso para utilizar o **Microsoft SQL Server** via Entity Framework Core.

## ⚙️ Configuração e Execução Locais

### Pré-requisitos
- .NET 10 SDK
- Microsoft SQL Server (LocalDB ou em contêiner Docker)

### Passos

1. **Clone o repositório e acesse a pasta raiz.**
2. **Atualize as credenciais no `appsettings.json` (no projeto FCG.API):**
   Verifique a connection string `"DefaultConnection"` e tenha certeza de que o SQL Server está acessível.
3. **Aplicar Migrations:**
   No diretório do projeto da API (`src/FCG.API`), execute o seguinte comando para criar a base de dados:
   ```bash
   dotnet ef database update --project ../FCG.Infrastructure
   ```
4. **Executar a API:**
   Ainda no diretório da API, execute o comando:
   ```bash
   dotnet run
   ```
5. **Acessar a documentação (Swagger):**
   Com o ambiente em modo de desenvolvimento, acesse a URL base (ex: `https://localhost:7123/swagger`) no navegador para testar os endpoints interativamente.

## 🔒 Autenticação

Para os endpoints protegidos, é necessário realizar o login ou registro para obter um token JWT.
No Swagger, utilize o botão "Authorize" no topo da página e insira seu token no formato: `Bearer <SEU_TOKEN>`.
