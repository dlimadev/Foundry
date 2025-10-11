// The code should be in English
using Castle.DynamicProxy;
using Foundry.Api.BuildingBlocks.Services;
using Foundry.Domain.Interfaces.Specifications;
using Foundry.Domain.Model;
using Foundry.Domain.Model.Attributes;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Foundry.Infrastructure.Interceptors
{
    /// <summary>
    /// An interceptor that adds a distributed caching layer to repository methods.
    /// It intercepts method calls, checks for the [Cacheable] attribute on the entity,
    /// and serves/stores data from/to the cache for read queries. It also reports the
    /// data source ("Cache" or "Database") via the IResponseMetaProvider.
    /// </summary>
    public class CachingInterceptor : IAsyncInterceptor
    {
        private readonly IDistributedCache _cache;
        private readonly IResponseMetaProvider _metaProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingInterceptor"/> class.
        /// </summary>
        /// <param name="cache">The distributed cache provider (e.g., Redis).</param>
        /// <param name="metaProvider">The scoped service used to communicate metadata back to the response pipeline.</param>
        public CachingInterceptor(IDistributedCache cache, IResponseMetaProvider metaProvider)
        {
            _cache = cache;
            _metaProvider = metaProvider;
        }

        /// <inheritdoc />
        public void InterceptSynchronous(IInvocation invocation)
        {
            // Caching is primarily for async read operations, so we bypass sync calls.
            invocation.Proceed();
        }

        /// <inheritdoc />
        public void InterceptAsynchronous(IInvocation invocation)
        {
            // This handles async methods that return a non-generic Task.
            // These methods typically don't return data to cache, so we just let them proceed.
            invocation.ReturnValue = HandlePassthroughAsync(invocation);
        }

        /// <inheritdoc />
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            // This is the main method for handling async methods that return data (Task<TResult>),
            // such as GetByIdAsync and ListAsync.
            invocation.ReturnValue = HandleCacheableMethodAsync<TResult>(invocation);
        }

        /// <summary>
        /// Contains the core caching logic for methods that return data.
        /// </summary>
        private async Task<TResult> HandleCacheableMethodAsync<TResult>(IInvocation invocation)
        {
            var entityType = GetEntityTypeFromInvocation(invocation);
            var cacheableAttribute = entityType?.GetCustomAttribute<CacheableAttribute>();

            // If the entity is not marked as [Cacheable], or if the method is not a read query, bypass caching.
            if (cacheableAttribute == null || !IsReadQuery(invocation.Method.Name))
            {
                invocation.Proceed();
                return await (Task<TResult>)invocation.ReturnValue;
            }

            var cacheKey = await GenerateCacheKey(entityType!, invocation);
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            if (cachedValue != null)
            {
                // --- CACHE HIT ---
                // Report that the data source was the cache.
                _metaProvider.Add("DataSource", "Cache");
                // Deserialize and return the cached value immediately.
                return JsonSerializer.Deserialize<TResult>(cachedValue)!;
            }

            // --- CACHE MISS ---
            // Report that we are going to the database.
            _metaProvider.Add("DataSource", "Database");

            // Proceed with the original method call to fetch data from the database.
            invocation.Proceed();
            var result = await (Task<TResult>)invocation.ReturnValue;

            // If the database returned a result, store it in the cache for next time.
            if (result != null)
            {
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheableAttribute.DurationInMinutes) };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);
            }

            return result;
        }

        /// <summary>
        /// A helper to simply execute and await methods that are not candidates for caching.
        /// </summary>
        private async Task HandlePassthroughAsync(IInvocation invocation)
        {
            invocation.Proceed();
            await (Task)invocation.ReturnValue;
        }

        /// <summary>
        /// Determines if a method is a read query based on its name.
        /// </summary>
        private bool IsReadQuery(string methodName) => methodName.StartsWith("Get") || methodName.StartsWith("List");

        /// <summary>
        /// Extracts the generic entity type from the repository method being invoked.
        /// </summary>
        private Type? GetEntityTypeFromInvocation(IInvocation invocation)
        {
            if (invocation.GenericArguments.Any())
            {
                return invocation.GenericArguments[0];
            }

            var targetType = invocation.TargetType;
            var genericInterface = targetType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Domain.Interfaces.Repositories.IGenericRepository<>));
            return genericInterface?.GetGenericArguments()[0];
        }

        /// <summary>
        /// Generates a unique and deterministic cache key for a method invocation.
        /// </summary>
        private async Task<string> GenerateCacheKey(Type entityType, IInvocation invocation)
        {
            var entityTypeName = entityType.Name;

            // Handle GetByIdAsync(guid)
            if (invocation.Method.Name.Contains("ById") && invocation.Arguments.Any())
            {
                return $"entity-{entityTypeName}-{invocation.Arguments[0]}";
            }

            // Handle ListAsync(ISpecification<T>) using the version token strategy
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

            // Fallback for other read methods
            return $"other-{entityTypeName}-{invocation.Method.Name}";
        }
    }
}