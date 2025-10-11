# Foundry Framework

`Foundry` � um DevPack opinativo e reutiliz�vel para construir aplica��es .NET 8 robustas, seguindo os princ�pios do Domain-Driven Design (DDD) e da Arquitetura Limpa.

## Prop�sito

O objetivo deste framework � acelerar o desenvolvimento, fornecendo uma base s�lida e test�vel para novas aplica��es, com "grades de prote��o" (guardrails) que guiam os desenvolvedores para as melhores pr�ticas de arquitetura.

## Projetos do Framework

- **`Foundry.Domain`**: O cora��o do framework. Cont�m abstra��es e classes base para um Modelo de Dom�nio Rico (EntityBase, ValueObject, IAggregateRoot), padr�es de persist�ncia (IUnitOfWork, IRepository, ISpecification), suporte a Event Sourcing (IEventStore, EventSourcedAggregateRoot) e padr�es transversais (IDomainEvent, INotificationHandler).
- **`Foundry.Application.Abstractions`**: Cont�m abstra��es reutiliz�veis para a camada de Aplica��o, como o padr�o de retorno `Result<T>`.
- **`Foundry.Infrastructure`**: Fornece implementa��es concretas e reutiliz�veis para os contratos do dom�nio, como `GenericRepository`, `UnitOfWork`, `MartenEventStore`, sistema de cache com Redis (Decorator) e logging com Serilog (Enrichers).
- **`Foundry.Api.BuildingBlocks`**: Cont�m componentes "plug-and-play" para a camada de API, como middlewares para tratamento de exce��es e `Correlation ID`.

## Principais Funcionalidades

- Arquitetura Limpa e DDD prontos para uso.
- Modelo de Dom�nio Rico com `EntityBase` (Auditoria, Concorr�ncia, Soft-Delete).
- Suporte a persist�ncia h�brida (CRUD via EF Core e Event Sourcing).
- Padr�o Repository e Specification para consultas de neg�cio desacopladas.
- Despacho de Eventos de Dom�nio com MediatR.
- Sistema de Notifica��o para valida��es.
- Cache distribu�do (Redis) com invalida��o autom�tica via eventos.
- Logging estruturado e seguro (Serilog) com mascaramento de dados sens�veis.
- Cliente HTTP resiliente com pol�ticas Polly (Retry/Circuit Breaker).