# Sistema de Compra Programada de Ações

## Visão Geral

Este projeto implementa um **Sistema de Compra Programada de Ações**, no qual clientes podem aderir a um plano de investimento recorrente baseado em uma cesta de ativos (Top Five).

O sistema permite:

- Cadastro de clientes no plano de investimento
- Processamento de aportes recorrentes
- Compra programada de ativos
- Distribuição dos ativos adquiridos para os clientes
- Controle de custódia
- Leitura de cotações do mercado

A solução foi construída com foco em **simplicidade, desacoplamento e evolução futura da arquitetura**.

---

# Arquitetura

## Monólito Modular

Para este desafio foi escolhida a arquitetura **Monolítica Modular**.

Essa decisão foi tomada porque o escopo do projeto **não exige a complexidade operacional de microserviços**, e utilizar esse tipo de arquitetura poderia introduzir **configuração e complexidade desnecessária**.

O monólito modular permite manter a aplicação simples, porém bem organizada. Mantive duas webapi, uma para cliente e outra para backoffice com o intuito de deixar desacoplado os serviços diferentes.

### Vantagens do Monólito Modular

- Simplicidade de desenvolvimento
- Menor complexidade de infraestrutura
- Melhor performance (sem comunicação via rede entre serviços)
- Debugging mais simples
- Organização por domínio
- Possibilidade de evoluir para microserviços no futuro

Apesar de ser um monólito, o sistema foi estruturado em **módulos bem definidos**, permitindo que no futuro partes do sistema possam ser extraídas para **microserviços**, caso necessário.

---

# Clean Architecture

O projeto também utiliza princípios da **Clean Architecture**, garantindo separação clara de responsabilidades entre as camadas do sistema.

Essa abordagem permite manter o **domínio da aplicação independente de frameworks e tecnologias externas**.

### Benefícios da Clean Architecture

- Baixo acoplamento entre camadas
- Melhor manutenção do código
- Maior organização da aplicação
- Possibilidade de trocar tecnologias sem afetar o domínio

---

## Estrutura das Camadas

### Core /Aplicação

Contém:

- Regras de negócio
- Serviços da aplicação
- Interfaces de serviços

---

### Infrastructure

Responsável por:

- Persistência no banco de dados
- Integração com Kafka
- Leitura de arquivos

---

### API

Responsável por:

- Controllers
- Exposição dos endpoints HTTP

---

# Processamento de Arquivo de Cotações

Para leitura do arquivo de cotações foi utilizada a biblioteca **FileHelpers**.

Essa biblioteca permite mapear arquivos texto diretamente para objetos.

### Vantagens

- Elimina a necessidade de `Substring` ou parsing manual
- Código mais legível
- Mapeamento declarativo usando atributos
- Menos propenso a erros
- Melhor manutenção
- Boa performance para leitura de arquivos estruturados

Isso torna o código de leitura de arquivos **mais simples**.

---

# Validação de Dias Úteis

Para validação de dias úteis foi utilizada a biblioteca **BrazilHoliday**.

Ela permite identificar automaticamente:

- Finais de semana
- Feriados nacionais brasileiros

### Benefícios

- Evita manutenção manual de calendários
- Código mais simples
- Regras de calendário confiáveis
- Facilita cálculo de próximos dias úteis

Essa validação é importante para garantir que as operações ocorram apenas em **dias de pregão**.

---

# Infraestrutura com Docker

O projeto utiliza **Docker** para execução dos serviços de infraestrutura.

Os seguintes componentes são executados em containers:

### MySQL

Responsável pela persistência dos dados da aplicação.

As Migrations são aplicadas no banco quando uma das Web APIs são executadas.

A conta Master é registrada no banco assim que a Web API Backoffice é executada.

### Kafka

Utilizado para comunicação assíncrona entre partes do sistema.

---

# Workers e Processamento Assíncrono

Inicialmente a arquitetura foi planejada para possuir **Worker Services separados** para:

- Cotação
- Motor de Compra Programada
- Motor de Rebalanceamento

Por isso, algumas partes da estrutura foram organizadas pensando nesse modelo.
Mas como os workers não foram testados foi atribuido um endpoint de cotação para fazer a leitura do arquivo.

Mesmo que neste momento o sistema esteja em um **monólito modular**, essa estrutura facilita no futuro:

- Extrair cada motor para um microserviço
- Escalar partes específicas do sistema
- Executar processamentos assíncronos isoladamente

---

# Diagrama de Relacionamento das Tabelas


```text
+----------------------+          +--------------------+
|   ClienteCadastro    | 1 : 1   |    ContaGrafica    |
+----------------------+----------+--------------------+
| Id (PK)              |          | Id (PK)            |
| Nome                 |          | NumeroConta        |
| Cpf                  |          | Tipo               |
| Email                |          | DataCriacao        |
| ValorMensal          |          | ClienteId (FK)     |
| Ativo                |          +--------------------+
| DataAdesao           |                  |
| DataSaida            |                  | 1 : N
+----------------------+                  |
                                          v
                               +------------------------+
                               | ContaCustodiaFilhote   |
                               +------------------------+
                               | Id (PK)                |
                               | ContaGraficaId (FK)    |
                               | Ticker                 |
                               | Quantidade             |
                               | PrecoMedio             |
                               | ValorAtual             |
                               | DataUltimaAtualizacao  |
                               +------------------------+

+--------------------+           +--------------------+
|    ContaMaster     | 1 : N     |    ItemCustodia    |
+--------------------+-----------+--------------------+
| Id (PK)            |           | Id (PK)            |
| NumeroConta        |           | Ticker             |
| Tipo               |           | Quantidade         |
+--------------------+           | PrecoMedio         |
                                 | ValorAtual         |
                                 | Origem             |
                                 | ContaMasterId (FK) |
                                 +--------------------+

+--------------------+           +--------------------+
|       Cesta        | 1 : N     |      ItemCesta     |
+--------------------+-----------+--------------------+
| Id (PK)            |           | Id (PK)            |
| Nome               |           | Ticker             |
| Ativa              |           | Percentual         |
| DataCriacao        |           | CotacaoAtual       |
| DataDesativacao    |           | CestaId (FK)       |
+--------------------+           +--------------------+

+--------------------+
|      Cotacao       |
+--------------------+
| Id (PK)            |
| Ticker             |
| DataPregao         |
| PrecoFechamento    |
| PrecoAbertura      |
| PrecoMaximo        |
| PrecoMinimo        |
| TipoMercado        |
| CodigoBDI          |
+--------------------+

+--------------------+           +--------------------+
|    OrdemCompra     | 1 : N     |   DetalheOrdem     |
+--------------------+-----------+--------------------+
| Id (PK)            |           | Id (PK)            |
| Ticker             |           | Tipo               |
| QuantidadeTotal    |           | Ticker             |
| PrecoUnitario      |           | Quantidade         |
| ValorTotal         |           | OrdemCompraId (FK) |
+--------------------+           +--------------------+

+--------------------+
|  ResiduoMaster     |
+--------------------+
| Id (PK)            |
| Ticker             |
| Quantidade         |
| OrdemCompraId (FK) |
+--------------------+

+------------------------+          +------------------------+
| DistribuicaoCliente    | 1 : N    |   AtivoDistribuido      |
+------------------------+----------+------------------------+
| Id (PK)                |          | Id (PK)                |
| ClienteId              |          | Ticker                 |
| Nome                   |          | Quantidade             |
| ValorAporte            |          | DistribuicaoClienteId  |
+------------------------+          +------------------------+
