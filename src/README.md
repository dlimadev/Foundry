# Exemplo 1: Mercado Financeiro (DDD Cl�ssico)

Esta aplica��o de exemplo demonstra como **usar** o framework `Foundry` para construir um sistema de mercado financeiro, seguindo uma abordagem de DDD cl�ssico com persist�ncia h�brida.

## Objetivo

O objetivo deste projeto � servir como um guia pr�tico para:
- Estruturar uma aplica��o em camadas (`Domain`, `Application`, `Infrastructure`, `Api`).
- Implementar entidades de neg�cio ricas (`Order`, `Portfolio`) que herdam de `Foundry.Domain`.
- Configurar e consumir os componentes do `Foundry.Infrastructure` (Unit of Work, Reposit�rios, etc.).
- Expor os casos de uso atrav�s de uma API Web.
- Demonstrar a implementa��o de padr�es de projeto cl�ssicos.

## Padr�es de Projeto Demonstrados

- **Repository & Unit of Work:** Utilizados para a persist�ncia do agregado `Portfolio`.
- **State:** Gerencia o ciclo de vida do agregado `Order`.
- **Chain of Responsibility:** Valida uma `Order` antes de uma transi��o de estado.
- **Specification:** Utilizado para criar consultas de neg�cio reutiliz�veis para `Stock`.
- **Composite:** A estrutura aninhada da entidade `Portfolio`.

## Como Executar

Este projeto � orquestrado pelo arquivo `docker-compose.yml` na raiz do reposit�rio. Para execut�-lo:
1. Garanta que o Docker Desktop esteja rodando.
2. Abra um terminal na raiz do reposit�rio.
3. Execute o comando: `docker-compose up --build`.
4. A API estar� acess�vel em `http://localhost:8080/swagger`.