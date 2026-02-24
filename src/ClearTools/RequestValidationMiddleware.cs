using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clear
{
    /// <summary>
    /// Options for request validation.
    /// </summary>
    public class RequestValidationOption
    {
        public RequestValidationOption(string validationKey, bool skipForDevelopment = false, bool skipForRootEndPoint = true, params string[] excludedPaths)
        {
            ValidationKey = validationKey;
            SkipForDevelopment = skipForDevelopment;
            SkipForRootEndPoint = skipForRootEndPoint;
            ExcludedPaths = excludedPaths ?? Array.Empty<string>();
        }

        /// <summary>
        /// The validation key.
        /// </summary>
        public string ValidationKey { get; set; }

        /// <summary>
        /// Whether to skip validation for development.
        /// </summary>
        public bool SkipForDevelopment { get; set; }

        /// <summary>
        /// Whether to skip validation for the root endpoint.
        /// </summary>
        public bool SkipForRootEndPoint { get; set; }

        /// <summary>
        /// Paths to exclude from validation (case-insensitive).
        /// </summary>
        public string[] ExcludedPaths { get; set; }
    }

    /// <summary>
    /// Middleware for request validation.
    /// </summary>
    public class RequestValidationMiddleware : IMiddleware
    {
        readonly string _key;
        readonly bool _skipDev = false;
        readonly bool _skipRoot = true;
        readonly string[] _excludedPaths;

        public RequestValidationMiddleware(string validationKey, bool skipForDevelopment = false, bool skipRoot = true, params string[] excludedPaths)
        {
            _key = validationKey;
            _skipDev = skipForDevelopment;
            _skipRoot = skipRoot;
            _excludedPaths = excludedPaths ?? Array.Empty<string>();
        }

        public RequestValidationMiddleware(RequestValidationOption option)
        {
            _key = option.ValidationKey;
            _skipDev = option.SkipForDevelopment;
            _skipRoot = option.SkipForRootEndPoint;
            _excludedPaths = option.ExcludedPaths;
        }

        /// <summary>
        /// Invokes the middleware to validate the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Check if path should be excluded from validation
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
            var isExcluded = _excludedPaths.Any(excluded =>
                path.Equals(excluded, StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));

            if (!((_skipRoot && context.Request.Path == "/") ||
                  (_skipDev && context.Request.Host.Host.Contains("localhost")) ||
                  isExcluded))
            {
                string key = context.Request.Headers["key"].ToString();

                if (RequestNotValid(key))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized request: Please add api validate to the header");
                    return;
                }
            }

            await next(context);
        }

        /// <summary>
        /// Checks if the request is not valid.
        /// </summary>
        /// <param name="key">The validation key from the request.</param>
        /// <returns>True if the request is not valid, otherwise false.</returns>
        bool RequestNotValid(string key) => key != _key;
    }

    /// <summary>
    /// Extension methods for adding and using the request validation middleware.
    /// </summary>
    public static class RequestValidationMiddlewareExtension
    {
        /// <summary>
        /// Adds the request validation middleware to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="validationKey">The validation key.</param>
        /// <param name="skipForDevelopment">Whether to skip validation for development.</param>
        /// <param name="skipRootEndPoint">Whether to skip validation for the root endpoint.</param>
        /// <param name="excludedPaths">Paths to exclude from validation (case-insensitive).</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestValidation(this IServiceCollection services,
            string validationKey, bool skipForDevelopment = false, bool skipRootEndPoint = true, params string[] excludedPaths)
        {
            services.AddSingleton(s => new RequestValidationMiddleware(validationKey, skipForDevelopment, skipRootEndPoint, excludedPaths));
            return services;
        }

        /// <summary>
        /// Uses the request validation middleware in the application builder.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseRequestValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}