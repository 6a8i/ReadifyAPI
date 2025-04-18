using Microsoft.Extensions.DependencyInjection;
using Readify.Infrastructure.Commons.DatabaseContexts.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Readify.Application.Features.Authentications.V1.Implementations;
using StackExchange.Redis;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System.Net;

namespace Readify.CrossCutting.DependencyInjection
{
    public static class IoCExtension
    {
        private readonly static AuthenticationAppServices auth; // This is only to get the explicit reference to the Application layer, so that the IoC container can be registered properly.
        public static IServiceCollection AddIoC(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAppServices().AddRepositories(configuration).AddFusionCacheConfigurations(configuration);
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

                if (implementationClass is not null)
                    services.AddScoped(interfaceClass, implementationClass);
            }

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReadifyDatabaseContext>(options =>
                options.UseSqlServer(configuration.GetSection("ConnectionStrings")["DefaultConnection"]));

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

        public static IServiceCollection AddFusionCacheConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar o Redis como cache distribuído

            // Configuração do Redis
            var redisConnectionString = configuration.GetSection("ConnectionStrings")["redis_connection_string"];
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString; // String de conexão do Redis
                options.InstanceName = "readify-api:"; // Prefixo opcional para as chaves do Redis
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints =  
                    {
                        {
                           redisConnectionString
                        }
                    },
                    AbortOnConnectFail = false,
                    ResolveDns = true,
                    ClientName = "Readify.Api",
                    ChannelPrefix = RedisChannel.Pattern("readify-api:"),
                };
            });

            services.AddFusionCacheSystemTextJsonSerializer(); // Adiciona o serializador System.Text.Json para o FusionCache

            // Configuração do Fusion Cache
            services.AddFusionCache()
                .WithDefaultEntryOptions(options =>
                {
                    options.Duration = TimeSpan.FromMinutes(30); // Duração padrão de 5 minutos
                    options.Priority = CacheItemPriority.NeverRemove; // Prioridade normal
                    options.AllowBackgroundBackplaneOperations = true; // Permite operações de backplane em segundo plano
                    options.AllowBackgroundDistributedCacheOperations = true; // Permite operações de cache distribuído em segundo plano 
                    options.DistributedCacheDuration = TimeSpan.FromMinutes(30); // Duração padrão de 5 minutos para o cache distribuído
                    options.SkipDistributedCacheWrite = false; // Não pula a gravação no cache distribuído
                    options.SkipDistributedCacheRead = false; // Não pula a leitura do cache distribuído
                })
                .WithRegisteredDistributedCache()
                .WithStackExchangeRedisBackplane(options =>
                {  // Usa o Redis como backplane
                    options.ConnectionMultiplexerFactory = async () => await ConnectionMultiplexer.ConnectAsync(redisConnectionString); // Usa o Redis como backplane
                    options.Configuration = redisConnectionString; // Configuração do Redis
                    options.ConfigurationOptions = new ConfigurationOptions
                    {
                        EndPoints = { redisConnectionString },
                        AbortOnConnectFail = false,
                        ResolveDns = true,
                        ClientName = "Readify.Api",
                        ChannelPrefix = RedisChannel.Pattern("readify-api:"),
                    };
                })
                .WithCacheKeyPrefix("readify-api") // Prefixo para as chaves do cache
                .TryWithRegisteredDistributedCache() // Tenta usar o cache distribuído registrado
                .WithRegisteredSerializer(); // Serializador padrão
            return services;
        }
    }
}
