# 🚀 Sistema de Gestão de Clientes (Customer Management System)

## 1. APRESENTAÇÃO DO PROJETO

Este projeto é um sistema de gestão de clientes desenvolvido com ASP.NET Core 9, seguindo os princípios do Domain-Driven Design (DDD). Ele oferece uma API RESTful para realizar operações CRUD (Create, Read, Update, Delete) em dados de clientes, incluindo informações de contato como e-mails, telefones, endereços e documentos.

### Objetivo Principal e Escopo
O objetivo principal deste projeto é demonstrar a aplicação prática de conceitos avançados de arquitetura de software, como DDD, padrões de projeto e Entity Framework Core, em um cenário de negócios comum: a gestão de dados de clientes. O escopo abrange a criação, leitura, atualização e exclusão de clientes e seus respectivos dados de contato, com foco na robustez do domínio.

### O que o Projeto Faz
O sistema permite:
- **Cadastrar novos clientes** com informações básicas e múltiplos contatos.
- **Consultar clientes** individualmente ou em lista.
- **Atualizar dados de clientes**, incluindo seus contatos.
- **Remover clientes** do sistema.
- Gerenciar **múltiplos e-mails, telefones e endereços** para um cliente, com a capacidade de designar um como principal.
- Armazenar **múltiplos documentos** para um cliente.

### Para Quem É
Este projeto é ideal para:
- **Estudantes e desenvolvedores** que desejam aprofundar seus conhecimentos em ASP.NET Core, Entity Framework Core e, principalmente, Domain-Driven Design.
- **Professores e instrutores** como material didático para demonstrar boas práticas de arquitetura e desenvolvimento de software.
- **Equipes de desenvolvimento** que buscam um exemplo claro de como estruturar uma aplicação com DDD.

### Por Que Foi Criado
Foi criado para servir como um template e um guia prático para a construção de aplicações robustas e escaláveis, enfatizando a importância de um domínio bem modelado e desacoplado da infraestrutura. Ele aborda desafios comuns como a persistência de Value Objects e a implementação de padrões de repositório de forma didática.

## 2. ARQUITETURA E DESIGN

A arquitetura do projeto segue o padrão de **Arquitetura em Camadas (Layered Architecture)**, com forte influência do **Domain-Driven Design (DDD)**. Isso garante uma separação clara de responsabilidades, facilitando a manutenção, testabilidade e escalabilidade da aplicação.

### Explicação Completa da Arquitetura em Camadas

#### 1. **CustomerManagement.Domain (Camada de Domínio)**
- **Coração da aplicação.** Contém a lógica de negócios, entidades, Value Objects, agregados e interfaces de repositório.
- **Independente de qualquer tecnologia de infraestrutura ou UI.** Não conhece banco de dados, frameworks web, etc.
- **Foco:** Modelar o problema de negócio de forma rica e expressiva.

#### 2. **CustomerManagement.Infrastructure (Camada de Infraestrutura)**
- **Responsável pela persistência de dados e outras preocupações técnicas.**
- Implementa as interfaces de repositório definidas na camada de Domínio.
- Utiliza Entity Framework Core para interagir com o banco de dados (SQL Server LocalDB).
- Contém configurações de mapeamento de entidades para o banco de dados.
- Add-Migration Initial -Context CustomersDbContext -Project CustomerManagement.Infrastructure.Data -StartupProject CustomerManagement.Api

#### 3. **CustomerManagement.API (Camada de Apresentação/Aplicação)**
- **Ponto de entrada da aplicação.** Expõe a funcionalidade de negócio através de uma API RESTful.
- Contém controladores (Controllers) que recebem requisições HTTP, orquestram as operações de domínio e retornam respostas HTTP.
- Utiliza DTOs (Data Transfer Objects) para desacoplar a API do modelo de domínio.
- Configura a injeção de dependência e o pipeline da aplicação (middleware).

#### 4. **CustomerManagement.Infrastructure.Tests (Camada de Testes de Infraestrutura)**
- Contém testes unitários para a implementação do repositório, garantindo que a persistência de dados funcione corretamente.

#### 5. **CustomerManagement.Domain.Tests (Camada de Testes de Domínio)**
- Contém testes unitários para as entidades e Value Objects do domínio, garantindo que a lógica de negócio esteja correta e robusta.

### Padrões de Projeto Utilizados

-   **Domain-Driven Design (DDD)**: Foco na modelagem do domínio de negócio, com linguagem ubíqua e conceitos de Aggregate Roots, Value Objects e Repositories.
-   **Repository Pattern**: Abstrai a lógica de persistência de dados, permitindo que a camada de domínio trabalhe com coleções de objetos sem se preocupar com os detalhes do armazenamento.
-   **Factory Pattern**: Utilizado nos métodos `Create` dos Value Objects e Aggregate Roots para encapsular a lógica de criação e validação, garantindo que os objetos sejam sempre criados em um estado válido.
-   **Value Object Pattern**: Objetos que representam um conceito descritivo no domínio, definidos pela sua composição de atributos e comparados por valor, não por identidade. São imutáveis.
-   **Aggregate Root Pattern**: Entidades que são a raiz de um cluster de objetos (Aggregate), garantindo a consistência transacional dentro do agregado. Todas as operações externas devem passar pela Aggregate Root.
-   **Dependency Injection (DI)**: Utilizado para gerenciar as dependências entre as camadas e componentes, promovendo o baixo acoplamento e a testabilidade.
-   **Fluent API (EF Core)**: Usada para configurar o mapeamento objeto-relacional no Entity Framework Core, permitindo mapear Value Objects complexos para o banco de dados.
-   **RESTful API**: A camada de API segue os princípios REST para comunicação entre cliente e servidor, utilizando verbos HTTP e URLs semânticas.

### Fluxo de Dados Completo

1.  **Requisição HTTP (API)**: Um cliente (ex: frontend, Postman) envia uma requisição HTTP (POST, GET, PUT, DELETE) para um endpoint da `CustomerManagement.API`.
2.  **Controller (API)**: O `CustomersController` recebe a requisição, valida os DTOs de entrada e, se necessário, converte-os para o formato esperado pelo domínio.
3.  **Serviço de Aplicação (API/Domínio)**: O Controller invoca métodos na camada de Domínio (através da interface do repositório) para executar a lógica de negócio.
4.  **Aggregate Root (Domínio)**: O `Customer` (Aggregate Root) executa as regras de negócio, manipula seus Value Objects (`Email`, `Phone`, `Address`, `Document`) e garante a consistência interna.
5.  **Repositório (Domínio/Infraestrutura)**: A interface `ICustomerRepository` é invocada. A implementação `SqlServerCustomerRepository` (na camada de Infraestrutura) traduz as operações de domínio em operações de banco de dados.
6.  **Entity Framework Core (Infraestrutura)**: O EF Core, usando o `CustomerDbContext` e as configurações da `CustomerConfiguration`, interage com o SQL Server LocalDB para persistir ou recuperar os dados.
7.  **Resposta (Infraestrutura/Domínio/API)**: Os dados são retornados do banco, convertidos de volta para objetos de domínio, e então para DTOs de resposta pela API, que são enviados de volta ao cliente como uma resposta HTTP.

### Diagrama ASCII da Arquitetura
