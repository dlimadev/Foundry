using Foundry.Application.Abstractions.Responses;

namespace Foundry.Application.Abstractions.Services
{
    /// <summary>
    /// A generic contract for basic CRUD application services.
    /// </summary>
    public interface IGenericCrudService<TDto, TCreateDto, TUpdateDto>
    {
        Task<Result<TDto>> GetByIdAsync(Guid id);
        Task<Result<IReadOnlyList<TDto>>> GetAllAsync();
        Task<Result<TDto>> CreateAsync(TCreateDto dto);
        Task<Result<TDto>> UpdateAsync(Guid id, TUpdateDto dto);
        Task<Result<object>> DeleteAsync(Guid id);
    }
}