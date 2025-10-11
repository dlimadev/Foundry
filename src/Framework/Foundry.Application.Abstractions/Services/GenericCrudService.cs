// The code should be in English
using Foundry.Application.Abstractions.Mappings;
using Foundry.Application.Abstractions.Responses;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Interfaces.Repositories;
using Foundry.Domain.Model;
using Foundry.Domain.Notifications;
using System.Net;

namespace Foundry.Application.Abstractions.Services
{
    /// <summary>
    /// An abstract base service for common CRUD operations, now integrated with the IMapper pattern.
    /// </summary>
    public abstract class GenericCrudService<TEntity, TDto, TCreateDto, TUpdateDto>
        : IGenericCrudService<TDto, TCreateDto, TUpdateDto>
        where TEntity : EntityBase, IAggregateRoot
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IGenericRepository<TEntity> Repository;
        protected readonly INotificationHandler Notifier;
        protected readonly IMapper<TEntity, TDto> Mapper;

        protected GenericCrudService(
            IUnitOfWork unitOfWork,
            IGenericRepository<TEntity> repository,
            INotificationHandler notifier,
            IMapper<TEntity, TDto> mapper)
        {
            UnitOfWork = unitOfWork;
            Repository = repository;
            Notifier = notifier;
            Mapper = mapper;
        }

        public virtual async Task<Result<TDto>> GetByIdAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                Notifier.AddError($"{typeof(TEntity).Name}.notFound", "Entity not found.");
                return Result<TDto>.Failure(Notifier.Notifications, HttpStatusCode.NotFound);
            }
            return Result<TDto>.Success(Mapper.Map(entity));
        }

        public virtual async Task<Result<IReadOnlyList<TDto>>> GetAllAsync()
        {
            var entities = await Repository.ListAllAsync();
            var dtos = entities.Select(Mapper.Map).ToList().AsReadOnly();
            return Result<IReadOnlyList<TDto>>.Success(dtos);
        }

        public async Task<Result<TDto>> CreateAsync(TCreateDto dto)
        {
            var entity = HandleCreate(dto);
            if (Notifier.HasErrors) return Result<TDto>.Failure(Notifier.Notifications);

            await Repository.AddAsync(entity);
            await UnitOfWork.CompleteAsync();

            return Result<TDto>.Success(Mapper.Map(entity), HttpStatusCode.Created);
        }

        public async Task<Result<TDto>> UpdateAsync(Guid id, TUpdateDto dto)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                Notifier.AddError($"{typeof(TEntity).Name}.notFound", "Entity not found.");
                return Result<TDto>.Failure(Notifier.Notifications, HttpStatusCode.NotFound);
            }

            HandleUpdate(entity, dto);
            if (Notifier.HasErrors) return Result<TDto>.Failure(Notifier.Notifications);

            await UnitOfWork.CompleteAsync();
            return Result<TDto>.Success(Mapper.Map(entity));
        }

        public async Task<Result<object>> DeleteAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                Notifier.AddError($"{typeof(TEntity).Name}.notFound", "Entity not found.");
                return Result<object>.Failure(Notifier.Notifications, HttpStatusCode.NotFound);
            }

            Repository.Remove(entity);
            await UnitOfWork.CompleteAsync();

            return Result<object>.Success(null!, HttpStatusCode.NoContent);
        }

        protected abstract TEntity HandleCreate(TCreateDto dto);
        protected abstract void HandleUpdate(TEntity entity, TUpdateDto dto);
    }
}