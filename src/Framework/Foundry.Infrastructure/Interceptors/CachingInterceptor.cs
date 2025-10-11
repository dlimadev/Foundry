using Castle.DynamicProxy;
using Foundry.Domain.Interfaces.Specifications;
using Foundry.Domain.Model;
using Foundry.Domain.Model.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Foundry.Infrastructure.Interceptors
{
    public class CachingInterceptor : IAsyncInterceptor
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CachingInterceptor(IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            invocation.Proceed();
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = HandlePassthroughAsync(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = HandleCacheableMethodAsync<TResult>(invocation);
        }

        // Handles methods that return Task<TResult> and applies caching logic if applicable.
        private async Task<TResult> HandleCacheableMethodAsync<TResult>(IInvocation invocation)
        {
            var entityType = GetEntityTypeFromInvocation(invocation);
            var cacheableAttribute = entityType?.GetCustomAttribute<CacheableAttribute>();

            if (cacheableAttribute == null || !IsReadQuery(invocation.Method.Name))
            {
                invocation.Proceed();
                return await (Task<TResult>)invocation.ReturnValue;
            }

            var cacheKey = await GenerateCacheKey(entityType!, invocation);
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            var httpContext = _httpContextAccessor.HttpContext;

            if (cachedValue != null)
            {
                if (httpContext != null) httpContext.Items["DataSource"] = "Cache";
                return JsonSerializer.Deserialize<TResult>(cachedValue)!;
            }

            if (httpContext != null) httpContext.Items["DataSource"] = "Database";

            invocation.Proceed();
            var result = await (Task<TResult>)invocation.ReturnValue;

            if (result != null)
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheableAttribute.DurationInMinutes) };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);
            }

            return result;
        }

        private async Task HandlePassthroughAsync(IInvocation invocation)
        {
            invocation.Proceed();
            await (Task)invocation.ReturnValue;
        }

        // Determines if the method is a read operation based on its name.
        private bool IsReadQuery(string methodName) => methodName.StartsWith("Get") || methodName.StartsWith("List");

        // Extracts the entity type from the invocation's generic arguments or target type.
        private Type? GetEntityTypeFromInvocation(IInvocation invocation)
        {
            if (invocation.GenericArguments.Any()) return invocation.GenericArguments[0];

            var targetType = invocation.TargetType;
            var genericInterface = targetType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Domain.Interfaces.Repositories.IGenericRepository<>));
            return genericInterface?.GetGenericArguments()[0];
        }

        // Generates a unique cache key based on the entity type, method name, and arguments.
        private async Task<string> GenerateCacheKey(Type entityType, IInvocation invocation)
        {
            var entityTypeName = entityType.Name;

            if (invocation.Method.Name.Contains("ById") && invocation.Arguments.Any())
            {
                return $"entity-{entityTypeName}-{invocation.Arguments[0]}";
            }

            if (invocation.Method.Name.Contains("List") && invocation.Arguments.FirstOrDefault() is ISpecification<EntityBase> spec)
            {
                var listVersionKey = $"list-version-{entityTypeName}";
                var versionToken = await _cache.GetStringAsync(listVersionKey);
                if (string.IsNullOrEmpty(versionToken))
                {
                    versionToken = Guid.NewGuid().ToString();
                    await _cache.SetStringAsync(listVersionKey, versionToken);
                }

                var specString = new StringBuilder();
                specString.Append(spec.Criteria?.ToString() ?? "all");
                specString.Append(string.Join("-", spec.Includes.Select(i => i.ToString())));
                specString.Append(spec.OrderBy?.ToString() ?? "");
                specString.Append(spec.OrderByDescending?.ToString() ?? "");

                var keyMaterial = $"{listVersionKey}:{versionToken}:{specString}";
                using var sha256 = SHA256.Create();
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyMaterial));

                return $"list-{entityTypeName}-{Convert.ToBase64String(hashBytes)}";
            }

            return $"other-{entityTypeName}-{invocation.Method.Name}";
        }
    }
}