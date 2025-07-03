using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ClearTools.Extensions
{
    enum ServiceLifetimeOption
    {
        Singleton,
        Scoped,
        Transient
    }

    public static class ServicesExtensions

    {
        public static IServiceCollection AddSingletonServicesByInterface<T>(this IServiceCollection services, Assembly? assembly = null) where T : class
        {
            return AddServicesByInterface<T>(services, ServiceLifetimeOption.Singleton, assembly);
        }

        public static IServiceCollection AddScopedServicesByInterface<T>(this IServiceCollection services, Assembly? assembly = null) where T : class
        {
            return AddServicesByInterface<T>(services, ServiceLifetimeOption.Scoped, assembly);
        }

        public static IServiceCollection AddTransientServicesByInterface<T>(this IServiceCollection services, Assembly? assembly = null) where T : class
        {
            return AddServicesByInterface<T>(services, ServiceLifetimeOption.Transient, assembly);
        }

        private static IServiceCollection AddServicesByInterface<T>(this IServiceCollection services, ServiceLifetimeOption lifetime, Assembly? assembly = null) where T : class
        {
            var serviceTypes = GetAllExportedTypes(assembly)
                .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t));

            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterfaces().FirstOrDefault(i => typeof(T).IsAssignableFrom(i));
                if (interfaceType != null)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetimeOption.Singleton:
                            services.AddSingleton(interfaceType, serviceType);
                            break;
                        case ServiceLifetimeOption.Scoped:
                            services.AddScoped(interfaceType, serviceType);
                            break;
                        case ServiceLifetimeOption.Transient:
                            services.AddTransient(interfaceType, serviceType);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                    }

                    services.AddScoped(interfaceType, serviceType);
                }
            }

            return services;
        }

        private static IEnumerable<Type> GetAllExportedTypes(Assembly? assembly = null)
        {
            return assembly == null
                ? AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.ExportedTypes)
                : assembly.ExportedTypes;
        }
    }
}