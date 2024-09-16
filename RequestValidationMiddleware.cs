using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Clear
{
    public class RequestValidationOption
    {
        public RequestValidationOption(string validationKey, bool skipForDevelopment = false, bool skipforRootEndPoint = true)
        {
            ValidationKey = validationKey;
            SkipForDevelopment = skipForDevelopment;
            SkipforRootEndPoint = skipforRootEndPoint;
        }

        public string ValidationKey { get; set; }
        public bool SkipForDevelopment { get; set; }
        public bool SkipforRootEndPoint { get; set; }
    }

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

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!((_skipRoot && context.Request.Path == "/") ||
                  (_skipDev && context.Request.Host.Host.Contains("localhost"))))
            {
                string key = context.Request.Headers["key"];

                if (RequestNotValid(key))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized request: Please add api validate to the header");
                    return;
                }
            }

            await next(context);
        }

        bool RequestNotValid(string key) => key != _key;
    }

    public static class RequestValidationMiddlewareExtension
    {
        public static IServiceCollection AddRequestValidation(this IServiceCollection services, string validationKey, bool skipForDevelopment = false, bool skipRootEndPoint = true)
        {
            services.AddSingleton(s => new RequestValidationMiddleware(validationKey, skipForDevelopment, skipRootEndPoint));
            return services;
        }

        public static IApplicationBuilder UseRequestValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestValidationMiddleware>();
        }
    }
}