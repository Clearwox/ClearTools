using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Clear
{
    /// <summary>
    /// Options for request validation.
    /// </summary>
    public class RequestValidationOption
    {
        public RequestValidationOption(string validationKey, bool skipForDevelopment = false, bool skipForRootEndPoint = true)
        {
            ValidationKey = validationKey;
            SkipForDevelopment = skipForDevelopment;
            SkipForRootEndPoint = skipForRootEndPoint;
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
    }

    /// <summary>
    /// Middleware for request validation.
    /// </summary>
    public class RequestValidationMiddleware : IMiddleware
    {
        readonly string _key;
        readonly bool _skipDev = false;
        readonly bool _skipRoot = true;

        public RequestValidationMiddleware(string validationKey, bool skipForDevelopment = false, bool skipRoot = true)
        {
            _key = validationKey;
            _skipDev = skipForDevelopment;
            _skipRoot = skipRoot;
        }

        public RequestValidationMiddleware(RequestValidationOption option)
        {
            _key = option.ValidationKey;
            _skipDev = option.SkipForDevelopment;
        }

        /// <summary>
        /// Invokes the middleware to validate the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!((_skipRoot && context.Request.Path == "/") ||
                  (_skipDev && context.Request.Host.Host.Contains("localhost"))))
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
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestValidation(this IServiceCollection services,
            string validationKey, bool skipForDevelopment = false, bool skipRootEndPoint = true)
        {
            services.AddSingleton(s => new RequestValidationMiddleware(validationKey, skipForDevelopment, skipRootEndPoint));
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