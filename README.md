# Sistema de Gerenciamento de Cursos e Matrículas

Aplicação de console desenvolvida em C# com .NET 10, SQLite e ADO.NET, organizada sob a arquitetura MVC (Model-View-Controller) combinada com o padrão Repository para acesso a dados. Projeto concebido para atividade acadêmica com foco em boas práticas de programação orientada a objetos, modelagem relacional e persistência robusta.

---

## 1. Tecnologias e Decisões Técnicas

- **Linguagem & Plataforma:** C# (.NET 10).
- **Banco de Dados Relacional:** SQLite (arquivo físico `cursos.db`).
- **Acesso a Dados:** ADO.NET puro com o provedor oficial `Microsoft.Data.Sqlite` (`SqliteConnection`, `SqliteCommand`, `SqliteDataReader`, `SqliteParameter`).
- **ORM:** Nenhum ORM utilizado (sem Entity Framework, Dapper ou equivalentes), respeitando rigorosamente o enunciado.
- **Segurança:** 100% das instruções SQL utilizam parâmetros para imunidade contra injeção de SQL (*SQL Injection*).
- **Gerenciamento de Recursos:** Uso de declarações `using var` em todas as conexões, comandos e leitores, garantindo liberação imediata de recursos do sistema operacional.

---

## 2. Entidades e Modelagem Orientada a Objetos

O domínio é composto por duas entidades relacionadas:

### `Curso`
- `Id` (int): Identificador único (chave primária).
- `Nome` (string): Nome do curso (único, não nulo).
- `CargaHoraria` (int): Duração em horas (deve ser > 0).
- `Descricao` (string): Detalhes sobre a ementa do curso.
- `Matriculas` (`List<Matricula>`): Coleção que representa o lado "1 para Muitos" (1:N) da relação no modelo de classes.

### `Matricula`
- `Id` (int): Identificador único (chave primária).
- `NomeAluno` (string): Nome do aluno matriculado.
- `DataMatricula` (DateTime): Data em que a matrícula foi efetivada.
- `CursoId` (int): Chave estrangeira que referencia o curso.
- `Curso` (`Curso?`): Referência de navegação ao curso associado (associação/agregação no modelo de classes).

---

## 3. Justificativa Funcional e Conceitual do Relacionamento

O enunciado exige demonstrar uma relação de **associação ou agregação** entre as entidades com a respectiva justificativa funcional:

1. **Classificação do Relacionamento:** Trata-se de uma **Agregação / Associação (1:N)**. Um curso atua como agregador de matrículas de estudantes.
2. **Independência de Ciclo de Vida do Todo (Curso):** A entidade `Curso` é a entidade-mestra do catálogo acadêmico. Ela possui ciclo de vida autônomo: uma instituição planeja, cadastra e disponibiliza um curso no catálogo antes mesmo de qualquer aluno se matricular. Portanto, a existência de um curso independe de haver matrículas vinculadas.
3. **Dependência Funcional da Parte (Matrícula):** A `Matrícula` expressa um vínculo formal entre um aluno e uma capacitação oferecida. No contexto acadêmico, não faz sentido a existência de uma matrícula "órfã" desprovida de curso. Por isso, o atributo `CursoId` é obrigatório (`NOT NULL`) e protegido por chave estrangeira.
4. **Política de Integridade (`ON DELETE RESTRICT`):** Como medida de segurança funcional e garantia de integridade histórica, um curso que já possua matrículas ativas **não pode ser excluído** enquanto houver alunos vinculados. A regra de negócio exige que as matrículas sejam transferidas ou canceladas previamente, evitando que dados acadêmicos sejam apagados inadvertidamente.

---

## 4. Modelagem Relacional e Script SQL

O banco de dados é configurado a partir do script `Scripts/schema.sql`, executado automaticamente na inicialização:

- **Tabela `Cursos`:**
  - Chave primária `INTEGER PRIMARY KEY AUTOINCREMENT`.
  - Restrição `COLLATE NOCASE UNIQUE` no nome do curso para evitar duplicidades independentemente de maiúsculas/minúsculas.
  - Restrição `CHECK (CargaHoraria > 0)` para validar coerência de horas.
- **Tabela `Matriculas`:**
  - Chave primária `INTEGER PRIMARY KEY AUTOINCREMENT`.
  - Chave estrangeira `FK_Matriculas_Cursos` referenciando `Cursos(Id)`.
  - Cláusula `ON DELETE RESTRICT` e `ON UPDATE CASCADE`.
  - Índice `IX_Matriculas_CursoId` para otimizar consultas filtradas por curso e junções relacionais (`INNER JOIN`).
- **Suporte a Foreign Keys no SQLite:** Ativado via connection string (`ForeignKeys = true`) em `Database.cs`.

---

## 5. Arquitetura da Aplicação

A solução adota a arquitetura **MVC (Model-View-Controller)** adaptada para aplicação de console, com a adição da camada **Data/Repositories**:

- **Models (`Models/`):** Representam as classes de domínio (`Curso.cs`, `Matricula.cs`) e o mapeamento das relações em memória.
- **Views (`Views/`):** Responsáveis por toda a interação com o usuário via terminal (`MenuView`, `CursoView`, `MatriculaView`, `EntradaView`), validações de formato e controle de exibição com proteção para ambientes sem console interativo.
- **Controllers (`Controllers/`):** Orquestram o fluxo de navegação, executam as regras de negócio, capturam exceções e acionam repositórios e visões (`CursoController`, `MatriculaController`).
- **Data / Repositories (`Data/`):** Concentram todo o código ADO.NET, isolando a tecnologia de persistência das demais camadas (`Database`, `CursoRepository`, `MatriculaRepository`).
- **Scripts (`Scripts/`):** Contém o script SQL declarativo para geração do banco.

---

## 6. Funcionalidades do Sistema

- **Gerenciar Cursos:**
  - Cadastrar curso com validação de unicidade e carga horária positiva.
  - Listar todos os cursos cadastrados.
  - Consultar curso por ID, exibindo os dados e a contagem de matrículas ativas.
  - Atualizar os dados de um curso.
  - Excluir curso (com proteção automática caso possua matrículas associadas).
- **Gerenciar Matrículas:**
  - Cadastrar matrícula vinculada a um curso existente (com listagem prévia dos cursos disponíveis).
  - Listar todas as matrículas cadastradas.
  - Consultar matrícula por ID, exibindo os dados da matrícula e o curso associado.
  - Atualizar dados do aluno, data e curso vinculado.
  - Excluir matrícula.
  - Listar matrículas filtradas por um determinado curso.
- **Relatório de Matrículas e Cursos:**
  - Consulta analítica utilizando `INNER JOIN` para exibir o nome do aluno, a data da matrícula e o nome oficial do curso associado.

---

## 7. Como Executar

### Pré-requisitos
- .NET 10 SDK instalado.
- Sistema operacional Windows, Linux ou macOS.

### Execução via Terminal
```bash
# Na pasta raiz do projeto:
dotnet build
dotnet run
```

O banco SQLite `cursos.db` será criado e inicializado automaticamente no diretório de execução da aplicação.
