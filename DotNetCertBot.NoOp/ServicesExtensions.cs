using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.NoOp
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddNoOpDnsProvider(this IServiceCollection services)
        {
            services.AddSingleton<IDnsProviderService, NoOpDnsProvider>();
            return services;
        }

        public static IServiceCollection AddNoOpAcme(this IServiceCollection services)
        {
            services.AddSingleton<IAcmeService, NoOpAcme>();
            return services;
        }
    }
}