# FCG (FIAP Cloud Games)

API REST para cadastro de usuarios, autenticacao, catalogo de jogos, fluxo de compra e biblioteca de jogos adquiridos.

O projeto usa Minimal APIs, Entity Framework Core com SQL Server, JWT Bearer, FluentValidation, Swagger e organizacao em camadas seguindo principios de Clean Architecture e DDD.

## Tecnologias

- .NET 10
- Entity Framework Core 10
- SQL Server
- JWT Bearer
- FluentValidation
- Swagger / OpenAPI
- xUnit e FluentAssertions
- Docker Compose

## Estrutura

- `src/FCG.API`: endpoints, middlewares, configuracao da API e composicao de dependencias.
- `src/FCG.Application`: services, DTOs, validadores, contratos e regras de aplicacao.
- `src/FCG.Domain`: entidades, enums, erros padronizados, Result Pattern e regras de dominio.
- `src/FCG.Infrastructure`: DbContext, migrations, repositories, Unit of Work e servicos de infraestrutura.
- `test/FCG.Test`: testes unitarios das principais regras.

## Regras Atendidas

### Cadastro de usuarios

O cadastro publico recebe nome, e-mail e senha.

A senha deve respeitar a politica minima:

- minimo de 8 caracteres;
- pelo menos uma letra;
- pelo menos um numero;
- pelo menos um caractere especial.

O e-mail e validado com FluentValidation.

### Autenticacao e autorizacao

A API usa JWT Bearer e possui dois perfis:

- `User`: acessa a plataforma, cria pedidos e consulta sua biblioteca.
- `Admin`: cadastra jogos, cria/desativa promocoes e administra usuarios.

Tokens incluem claims de id, nome, e-mail e role. Os logs estruturados incluem escopo com `UserId` e `UserEmail` para usuarios autenticados.

### Biblioteca de jogos adquiridos

O fluxo atual de aquisicao funciona por pedidos:

1. O usuario consulta o catalogo de jogos.
2. O usuario cria um pedido com um ou mais jogos.
3. O pagamento do pedido e aprovado.
4. Os jogos do pedido sao adicionados a biblioteca do usuario.

Usuarios nao podem consultar ou pagar pedidos de outros usuarios. Admins podem acessar pedidos para administracao.

### Administracao de usuarios

Endpoints protegidos por role `Admin` permitem:

- cadastrar usuarios com role `User` ou `Admin`;
- listar usuarios paginados, incluindo usuarios inativos;
- consultar usuario por id, incluindo usuarios inativos;
- alterar role do usuario.
- inativar usuarios por exclusao logica;
- reativar usuarios inativos.

O cadastro publico sempre cria usuarios com role `User`.

O usuario default `fgc_admin@admin.com` nao pode ser inativado pela API.
Usuarios inativos nao aparecem nas buscas comuns nem conseguem autenticar, mas continuam visiveis para Admin.

## Usuario Administrador Seedado

A aplicacao cria automaticamente um administrador no warmup, apos aplicar migrations.

```text
Email: fgc_admin@admin.com
Senha local padrao: Adm!n123
Role: Admin
```

A senha do admin e configuravel pela variavel de ambiente:

```text
Admin_Password
```

Essa senha tambem precisa seguir a politica forte: minimo de 8 caracteres, letras, numeros e caracteres especiais. Se uma senha fraca for configurada, a aplicacao falha no startup.

## Variaveis de Ambiente

```text
ConnectionStrings__DefaultConnection
chave_secreta
Admin_Password
Jwt_Key
Jwt_Issuer
Jwt_Audience
```

## Executando com Docker

```bash
docker-compose up --build
```

A API fica disponivel em:

```text
http://localhost:5169
```

O Swagger fica disponivel em ambiente de desenvolvimento:

```text
http://localhost:5169/swagger
```

## Executando Localmente

Suba o SQL Server localmente ou via Docker e execute:

```bash
dotnet run --project src/FCG.API
```

As migrations e o seed do admin sao aplicados automaticamente no warmup da aplicacao.

## Principais Endpoints

### Autenticacao

- `POST /api/auth/register`
- `POST /api/auth/login`

### Jogos

- `GET /api/games`
- `GET /api/games/{id}`
- `POST /api/games` - Admin

### Pedidos

- `POST /api/orders`
- `GET /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders/{id}/pay`

### Biblioteca

- `GET /api/library`

### Promocoes

- `GET /api/promotions`
- `POST /api/promotions` - Admin
- `DELETE /api/promotions/{id}` - Admin

### Administracao de Usuarios

- `POST /api/admin/users` - Admin
- `GET /api/admin/users` - Admin
- `GET /api/admin/users/{id}` - Admin
- `PATCH /api/admin/users/{id}/role` - Admin
- `DELETE /api/admin/users/{id}` - Admin, inativa usuario, exceto usuario default
- `PATCH /api/admin/users/{id}/reactivate` - Admin, reativa usuario

Exemplo de body para criar usuario Admin:

```json
{
  "name": "Admin 2",
  "email": "admin2@email.com",
  "password": "Adm!n123",
  "role": "Admin"
}
```

Exemplo de body para alterar role:

```json
{
  "role": "Admin"
}
```

## Testes

```bash
dotnet test
```

Para executar com cobertura das camadas de regras de negocio:

```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Os testes cobrem validacao de senha/e-mail, politica de senha forte, regras de dominio de pedidos, autorizacao por dono do pedido, fluxo de pedidos, promocoes, catalogo, administracao de usuarios e inativacao logica.
