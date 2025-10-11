# Foundry Framework

`Foundry` é um DevPack opinativo e reutilizável para construir aplicações .NET 8 robustas, seguindo os princípios do Domain-Driven Design (DDD) e da Arquitetura Limpa.

## Propósito

O objetivo deste framework é acelerar o desenvolvimento, fornecendo uma base sólida e testável para novas aplicações, com "grades de proteção" (guardrails) que guiam os desenvolvedores para as melhores práticas de arquitetura.

## Projetos do Framework

- **`Foundry.Domain`**: O coração do framework. Contém abstrações e classes base para um Modelo de Domínio Rico (EntityBase, ValueObject, IAggregateRoot), padrões de persistência (IUnitOfWork, IRepository, ISpecification), suporte a Event Sourcing (IEventStore, EventSourcedAggregateRoot) e padrões transversais (IDomainEvent, INotificationHandler).
- **`Foundry.Application.Abstractions`**: Contém abstrações reutilizáveis para a camada de Aplicação, como o padrão de retorno `Result<T>`.
- **`Foundry.Infrastructure`**: Fornece implementações concretas e reutilizáveis para os contratos do domínio, como `GenericRepository`, `UnitOfWork`, `MartenEventStore`, sistema de cache com Redis (Decorator) e logging com Serilog (Enrichers).
- **`Foundry.Api.BuildingBlocks`**: Contém componentes "plug-and-play" para a camada de API, como middlewares para tratamento de exceções e `Correlation ID`.

## Principais Funcionalidades

- Arquitetura Limpa e DDD prontos para uso.
- Modelo de Domínio Rico com `EntityBase` (Auditoria, Concorrência, Soft-Delete).
- Suporte a persistência híbrida (CRUD via EF Core e Event Sourcing).
- Padrão Repository e Specification para consultas de negócio desacopladas.
- Despacho de Eventos de Domínio com MediatR.
- Sistema de Notificação para validações.
- Cache distribuído (Redis) com invalidação automática via eventos.
- Logging estruturado e seguro (Serilog) com mascaramento de dados sensíveis.
- Cliente HTTP resiliente com políticas Polly (Retry/Circuit Breaker).