# 🚀 Minimal API (.NET 9.0) - Gerenciamento de Veículos
Este repositório contém uma Minimal API desenvolvida em .NET 9.0 para gerenciar o cadastro de veículos e administradores. O projeto utiliza Entity Framework Core para persistência de dados (MySQL), Autenticação JWT e Swagger para documentação.

## 🔗 Demo Online (Exemplo)
O projeto está atualmente em deploy na AWS e pode ser acessado publicamente:

- Base URL: http://ec2-18-228-228-41.sa-east-1.compute.amazonaws.com

- Documentação Swagger: Acesse http://ec2-18-228-228-41.sa-east-1.compute.amazonaws.com/swagger/index.html

## ⚙️ Tecnologias Utilizadas
- Runtime: .NET 9.0 (SDK/Runtime)

- Web Server: Kestrel, exposto via Nginx (Reverse Proxy)

- Banco de Dados: MySQL (via Pomelo.EntityFrameworkCore.MySql)

- ORM: Entity Framework Core (EF Core)

- Segurança: JWT Bearer Authentication

- Testes: MSTest (Projeto TEST/Test.csproj)

## 🔒 Autenticação e Perfis
O acesso a todos os endpoints, exceto o de Login e Home, exige autenticação JWT.

**Perfis de Acesso**
O sistema utiliza dois perfis principais para autorização (Authorization):

- Admin: Acesso total (CRUD completo em Admins e Vehicles).

- Editor: Apenas criação, leitura e atualização de veículos (Vehicles).

**Credenciais Padrão (Seed Inicial)**
O banco de dados é inicializado com um administrador padrão:

- Email: administrador@teste.com

- Senha: 123456

- Perfil: Admin

## 🛠️ Configuração e Execução Local
**Pré-requisitos**
1. .NET 9.0 SDK instalado.

2. Servidor MySQL ativo.

**1. Configurar o Banco de Dados**
- Localize: Edite o arquivo API/appsettings.json para configurar a Connection String do MySQL.
```bash

"ConnectionStrings": {
  "MySql": "Server=localhost;Database=minimal_api;Uid=root;Pwd=sua_senha;" 
}
```

**2. Rodar as Migrations (EF Core)**
Certifique-se de ter a ferramenta dotnet-ef instalada globalmente.

No diretório API (minimal-api/API/):

```bash

dotnet ef database update
```

Este comando criará as tabelas *Admins* e *Vehicles* e inserirá o administrador padrão.

**3. Iniciar a Aplicação**
A aplicação está configurada para iniciar na porta 5231.

No diretório API (minimal-api/API/):

```bash

dotnet run
```

A API será executada e estará acessível em http://localhost:5231 (ou no link público ipv4).

## 🧪 Executando os Testes
O projeto inclui testes de unidade e integração no projeto TEST/Test.csproj.

No diretório TEST (minimal-api/TEST/):

```bash

dotnet test
```
Os testes de integração utilizam uma conexão separada (minimal_api_test) e usam mocks para serviços, como o AdminServiceMock, para isolar a lógica.
