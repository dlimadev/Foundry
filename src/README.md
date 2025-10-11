# Exemplo 1: Mercado Financeiro (DDD Clássico)

Esta aplicação de exemplo demonstra como **usar** o framework `Foundry` para construir um sistema de mercado financeiro, seguindo uma abordagem de DDD clássico com persistência híbrida.

## Objetivo

O objetivo deste projeto é servir como um guia prático para:
- Estruturar uma aplicação em camadas (`Domain`, `Application`, `Infrastructure`, `Api`).
- Implementar entidades de negócio ricas (`Order`, `Portfolio`) que herdam de `Foundry.Domain`.
- Configurar e consumir os componentes do `Foundry.Infrastructure` (Unit of Work, Repositórios, etc.).
- Expor os casos de uso através de uma API Web.
- Demonstrar a implementação de padrões de projeto clássicos.

## Padrões de Projeto Demonstrados

- **Repository & Unit of Work:** Utilizados para a persistência do agregado `Portfolio`.
- **State:** Gerencia o ciclo de vida do agregado `Order`.
- **Chain of Responsibility:** Valida uma `Order` antes de uma transição de estado.
- **Specification:** Utilizado para criar consultas de negócio reutilizáveis para `Stock`.
- **Composite:** A estrutura aninhada da entidade `Portfolio`.

## Como Executar

Este projeto é orquestrado pelo arquivo `docker-compose.yml` na raiz do repositório. Para executá-lo:
1. Garanta que o Docker Desktop esteja rodando.
2. Abra um terminal na raiz do repositório.
3. Execute o comando: `docker-compose up --build`.
4. A API estará acessível em `http://localhost:8080/swagger`.