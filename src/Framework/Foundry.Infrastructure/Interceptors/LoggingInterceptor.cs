// The code should be in English
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Foundry.Infrastructure.Interceptors
{
    public class LoggingInterceptor : IAsyncInterceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;
        private readonly ActivitySource _activitySource;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger, ActivitySource activitySource)
        {
            _logger = logger;
            _activitySource = activitySource;
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            ExecuteSync(invocation);
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = ExecuteAsync(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = ExecuteAsync<TResult>(invocation);
        }

        private void ExecuteSync(IInvocation invocation)
        {
            var methodName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
            using var activity = _activitySource.StartActivity(methodName, ActivityKind.Internal);
            using (_logger.BeginScope("Processing {MethodName} with arguments {@MethodArguments}", methodName, invocation.Arguments))
            {
                _logger.LogInformation("[START] {MethodName}", methodName);
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    invocation.Proceed();
                    stopwatch.Stop();
                    _logger.LogInformation("[END] {MethodName} finished successfully in {ElapsedMilliseconds}ms.", methodName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogExceptionAndSetActivityError(activity, methodName, ex, stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
        }

        private async Task ExecuteAsync(IInvocation invocation)
        {
            var methodName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
            using var activity = _activitySource.StartActivity(methodName, ActivityKind.Internal);
            using (_logger.BeginScope("Processing {MethodName} with arguments {@MethodArguments}", methodName, invocation.Arguments))
            {
                _logger.LogInformation("[START] {MethodName}", methodName);
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    invocation.Proceed();
                    await (Task)invocation.ReturnValue;
                    stopwatch.Stop();
                    _logger.LogInformation("[END] {MethodName} finished successfully in {ElapsedMilliseconds}ms.", methodName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogExceptionAndSetActivityError(activity, methodName, ex, stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
        }

        private async Task<TResult> ExecuteAsync<TResult>(IInvocation invocation)
        {
            var methodName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
            using var activity = _activitySource.StartActivity(methodName, ActivityKind.Internal);
            using (_logger.BeginScope("Processing {MethodName} with arguments {@MethodArguments}", methodName, invocation.Arguments))
            {
                _logger.LogInformation("[START] {MethodName}", methodName);
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    invocation.Proceed();
                    var result = await (Task<TResult>)invocation.ReturnValue;
                    stopwatch.Stop();
                    _logger.LogInformation("[END] {MethodName} finished successfully in {ElapsedMilliseconds}ms.", methodName, stopwatch.ElapsedMilliseconds);
                    return result;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogExceptionAndSetActivityError(activity, methodName, ex, stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
        }

        private void LogExceptionAndSetActivityError(Activity? activity, string methodName, Exception ex, long elapsedMs)
        {
            _logger.LogError(ex, "[ERROR] Exception in {MethodName} after {ElapsedMilliseconds}ms.", methodName, elapsedMs);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection { { "exception.type", ex.GetType().Name }, { "exception.message", ex.Message }, { "exception.stacktrace", ex.ToString() }, }));
        }
    }
}