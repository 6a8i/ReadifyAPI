using Microsoft.Extensions.DependencyInjection;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Readify.CrossCutting.DependencyInjection
{
    public static class IoCExtension
    {
        public static IServiceCollection AddIoC(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAppServices();
            services.AddRepositories(configuration);
            return services;
        }

        private static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            IEnumerable<Type> implementationClasses = AppDomain.CurrentDomain
                                                                .Load("Readify.Application")
                                                                .GetTypes()
                                                                    .Where(t => t.IsClass && 
                                                                                t.Namespace is not null && 
                                                                                t.Name.Contains("AppServices", StringComparison.InvariantCultureIgnoreCase));

            IEnumerable<Type> interfaceClasses = AppDomain.CurrentDomain
                                                                .Load("Readify.Application")
                                                                .GetTypes()
                                                                    .Where(t => t.IsInterface && 
                                                                                t.Namespace is not null && 
                                                                                t.Name.Contains("AppServices", StringComparison.InvariantCultureIgnoreCase));
            foreach (Type interfaceClass in interfaceClasses)
            {
                Type? implementationClass = implementationClasses.FirstOrDefault(t => t.Name == interfaceClass.Name[1..]);
                
                if(implementationClass is not null)
                    services.AddScoped(interfaceClass, implementationClass);
            }

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<ReadifyDatabaseContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            IEnumerable<Type> implementationClasses = AppDomain.CurrentDomain
                                                                .Load("Readify.Infrastructure")
                                                                .GetTypes()
                                                                    .Where(t => t.IsClass &&
                                                                                t.Namespace is not null &&
                                                                                t.Name.Contains("Repository", StringComparison.InvariantCultureIgnoreCase));
            IEnumerable<Type> interfaceClasses = AppDomain.CurrentDomain
                                                                .Load("Readify.Application")
                                                                .GetTypes()
                                                                    .Where(t => t.IsInterface &&
                                                                                t.Namespace is not null &&
                                                                                t.Name.Contains("Repository", StringComparison.InvariantCultureIgnoreCase));
            foreach (Type interfaceClass in interfaceClasses)
            {
                Type? implementationClass = implementationClasses.FirstOrDefault(t => t.Name == interfaceClass.Name[1..]);

                if (implementationClass is not null)
                    services.AddScoped(interfaceClass, implementationClass);
            }

            return services;
        }
    }
}
