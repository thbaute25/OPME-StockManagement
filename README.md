# 🏥 Sistema de Gestão de Estoque de Materiais Cirúrgicos (OPME)

## 📘 Visão Geral do Projeto

O **Sistema de Gestão de Estoque de Materiais Cirúrgicos (OPME)** é uma solução completa desenvolvida em **.NET 8** com **Clean Architecture** para gestão e controle inteligente de materiais cirúrgicos (OPME – Órteses, Próteses e Materiais Especiais).

### 🎯 Objetivos

A aplicação busca resolver problemas críticos do gerenciamento manual ou descentralizado de materiais cirúrgicos:

- ✅ **Falta de visibilidade** sobre estoques críticos
- ✅ **Perdas financeiras** por vencimento de produtos
- ✅ **Dificuldade de prever** demandas futuras
- ✅ **Processos lentos** de reposição e auditoria

### 💡 Solução Proposta

Sistema centralizado, seguro e automatizado que oferece:


- 📦 **Controle completo** de entradas, saídas e uso de produtos
- 🔔 **Alertas automáticos** de itens críticos e baixo estoque
- 🔍 **Busca avançada** com paginação, ordenação e filtros
- 🌐 **API RESTful** com HATEOAS para descoberta automática de recursos
- 🖥️ **Interface Web MVC** completa para gestão visual com Bootstrap 5
- 📝 **Documentação automática** via Swagger/OpenAPI

### 📈 Progresso e Funcionalidades Implementadas

#### ✅ Arquitetura e Infraestrutura
- ✅ Clean Architecture com 4 camadas bem definidas
- ✅ Entity Framework Core com SQLite
- ✅ Migrations e seed automático (DbInitializer)
- ✅ Repository Pattern e Unit of Work
- ✅ Dependency Injection configurada

#### ✅ API RESTful Completa
- ✅ 6 Controllers API com CRUD completo:
  - `ProductsController`, `SuppliersController`, `StockController`
  - `BrandsController`, `StockOutputsController`, `SupplierConfigurationsController`
- ✅ HATEOAS em todas as respostas DTO
- ✅ 3 rotas de busca avançada com paginação, ordenação e filtros
- ✅ Validações via FluentValidation
- ✅ Documentação Swagger completa

#### ✅ Interface Web MVC
- ✅ 3 Controllers MVC completos com 15 views implementadas
- ✅ ViewModels com Data Annotations e validação client-side/server-side
- ✅ Layout responsivo Bootstrap 5 com tema customizado
- ✅ Rotas padrão e rotas personalizadas configuradas

#### ✅ Qualidade e Performance
- ✅ Logging estruturado implementado em todos os serviços
- ✅ Transações (Unit of Work) para garantir integridade de dados
- ✅ Eager Loading para evitar N+1 queries
- ✅ Validações robustas (FluentValidation + Data Annotations)

#### ✅ Status de Compilação
- ✅ **Projeto compila sem erros críticos**
- ✅ Todas as dependências restauradas
- ✅ Migrations aplicadas automaticamente
- ✅ Banco de dados criado automaticamente na primeira execução

---

## 🚀 Instalação e Configuração

### 📋 Pré-requisitos

Certifique-se de ter instalado:

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (versão 8.0 ou superior)
2. **Git** (para clonar o repositório)
3. **Navegador web** (Chrome, Firefox, Edge, etc.)

### 🔧 Passo a Passo para Instalação

#### **1. Clonar o Repositório**

```bash
git clone https://github.com/thbaute25/OPME-StockManagement.git
cd OPME-StockManagement
```

#### **2. Verificar Instalação do .NET**

```bash
dotnet --version
```

Deve retornar a versão 8.0.x ou superior.

#### **3. Restaurar Dependências**

```bash
dotnet restore
```

Este comando baixa todos os pacotes NuGet necessários.

#### **4. Verificar Estrutura do Projeto**

```bash
dotnet sln list
```

Deve listar todos os 4 projetos da solução:
- `OPME.StockManagement.Domain`
- `OPME.StockManagement.Application`
- `OPME.StockManagement.Infrastructure`
- `OPME.StockManagement.WebAPI`

#### **5. Verificar Compilação**

```bash
dotnet build
```

O projeto deve compilar sem erros críticos. Warnings de nullable podem existir, mas não impedem a execução.

#### **6. Executar a Aplicação**

```bash
dotnet run --project src/OPME.StockManagement.WebAPI --urls "http://localhost:5002"
```

A aplicação será iniciada e estará disponível em:
- **Interface Web**: http://localhost:5002
- **API Swagger**: http://localhost:5002/swagger
- **API Base**: http://localhost:5002/api

#### **7. Verificar Funcionamento**

1. Abra o navegador em http://localhost:5002
2. Você deve ver a página inicial com cards de navegação
3. Acesse http://localhost:5002/swagger para ver a documentação da API
4. O banco de dados SQLite será criado automaticamente na primeira execução
5. Dados de exemplo serão inseridos automaticamente pelo `DbInitializer`

### 🔄 Configurações Avançadas

#### **Alterar Porta da Aplicação**

Edite `Program.cs` ou use o parâmetro `--urls`:

```bash
dotnet run --project src/OPME.StockManagement.WebAPI --urls "http://localhost:5000"
```

#### **Alterar Localização do Banco de Dados**

Edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:/MeusDados/OPMEStockManagement.db"
  }
}
```

---

## 🗄️ Banco de Dados

### Configuração

- **Tipo**: SQLite (banco de arquivo)
- **Arquivo**: `OPMEStockManagement.db`
- **Localização**: `src/OPME.StockManagement.WebAPI/`

A connection string está configurada no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=OPMEStockManagement.db"
  }
}
```

**Observações:**
- O banco é criado automaticamente na primeira execução
- Não é necessário configurar servidor de banco de dados
- O banco é inicializado com dados de exemplo automaticamente (6 fornecedores, 4 marcas, 5 produtos)

---

## 🌐 Endpoints da API

### 🔵 Fornecedores (Suppliers)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/Suppliers` | Listar todos os fornecedores |
| GET | `/api/Suppliers/{id}` | Obter fornecedor por ID |
| POST | `/api/Suppliers` | Criar novo fornecedor |
| PUT | `/api/Suppliers/{id}` | Atualizar fornecedor |
| DELETE | `/api/Suppliers/{id}` | Excluir fornecedor |
| PATCH | `/api/Suppliers/{id}/toggle-status` | Alternar status ativo/inativo |
| POST | `/api/suppliers/search` | Busca avançada com filtros e paginação |

### 🟢 Produtos (Products)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/Products` | Listar todos os produtos |
| GET | `/api/Products/active` | Listar apenas produtos ativos |
| GET | `/api/Products/{id}` | Obter produto por ID |
| POST | `/api/Products` | Criar novo produto |
| PUT | `/api/Products/{id}` | Atualizar produto |
| DELETE | `/api/Products/{id}` | Excluir produto |
| PATCH | `/api/Products/{id}/toggle-status` | Alternar status ativo/inativo |
| POST | `/api/products/search` | Busca avançada com filtros e paginação |

### 🔴 Estoque (Stock)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/Stock` | Listar todo o estoque |
| GET | `/api/Stock/low-stock?minQuantity={qtd}` | Estoque baixo (padrão: 10) |
| GET | `/api/Stock/product/{productId}` | Estoque de um produto específico |
| POST | `/api/Stock/product/{productId}/add` | Adicionar quantidade ao estoque |
| POST | `/api/Stock/product/{productId}/reduce` | Reduzir quantidade do estoque |
| PUT | `/api/Stock/product/{productId}` | Definir quantidade exata |
| POST | `/api/stock/search` | Busca avançada com filtros e paginação |

### 🌐 Rotas MVC (Interface Web)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/` | Página inicial (Dashboard) |
| GET | `/ProductsMvc` | Listar produtos |
| GET | `/ProductsMvc/Create` | Formulário de criação de produto |
| GET | `/ProductsMvc/Edit/{id}` | Formulário de edição de produto |
| GET | `/ProductsMvc/Details/{id}` | Detalhes do produto |
| GET | `/SuppliersMvc` | Listar fornecedores |
| GET | `/SuppliersMvc/Create` | Formulário de criação de fornecedor |
| GET | `/SuppliersMvc/Edit/{id}` | Formulário de edição de fornecedor |
| GET | `/StockMvc` | Visualizar estoque |
| GET | `/StockMvc/LowStock` | Estoque baixo |

---

## 🛠️ Tecnologias Utilizadas

### **Backend**
- **.NET 8** - Framework principal
- **ASP.NET Core MVC** - Para views web
- **ASP.NET Core Web API** - Para API REST
- **Entity Framework Core** - ORM para banco de dados
- **SQLite** - Banco de dados relacional

### **Validação e Documentação**
- **FluentValidation** - Validações robustas em DTOs
- **Swagger/OpenAPI** - Documentação automática e interativa da API
- **Data Annotations** - Validação de ViewModels

### **Frontend (Views)**
- **Bootstrap 5.3.0** - Framework CSS
- **Bootstrap Icons 1.11.1** - Ícones
- **jQuery** - Manipulação DOM e validação

### **Arquitetura e Padrões**
- **Clean Architecture** - Separação de responsabilidades
- **Repository Pattern** - Acesso a dados com Eager Loading
- **Unit of Work** - Transações e consistência de dados
- **DTO Pattern** - Transferência de dados
- **Dependency Injection** - Inversão de controle

---

## 📝 Recursos Adicionais

### **HATEOAS (Hypermedia)**
Todas as respostas da API incluem links navegáveis para operações relacionadas.

### **Busca com Paginação**
As rotas de busca retornam resultados paginados com informações de página atual, total de páginas e navegação.

### **Tratamento de Erros**
A API retorna códigos HTTP apropriados:
- **200 OK**: Operação bem-sucedida
- **201 Created**: Recurso criado
- **204 No Content**: Operação bem-sucedida sem conteúdo
- **400 Bad Request**: Dados inválidos
- **404 Not Found**: Recurso não encontrado
- **409 Conflict**: Conflito (ex: CNPJ duplicado)

---

## 👥 Equipe

- **Thomas Henrique Baute** – RM560649
- **Gabriel Dos Santos** - RM560812
- **Bruno Tizer** - RM569999

---
