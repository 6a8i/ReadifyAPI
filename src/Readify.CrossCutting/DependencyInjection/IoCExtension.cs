using Microsoft.Extensions.DependencyInjection;
using Readify.Application.Features.Users.V1;
using Readify.Application.Features.Users.V1.Implementations;
using Readify.Application.Features.Users.V1.Infrastructure.IRepositories;
using Readify.Infrastructure.Contexts.Users.V1.Repositories;

namespace Readify.CrossCutting.DependencyInjection
{
    public static class IoCExtension
    {
        public static IServiceCollection AddIoC(this IServiceCollection services)
        {
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersAppServices, UsersAppServices>();
            return services;
        }
    }
}
