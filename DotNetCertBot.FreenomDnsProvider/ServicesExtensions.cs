using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.FreenomDnsProvider
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddFreenomDnsProvider(this IServiceCollection services)
        {
            services.AddSingleton<IDnsProviderService, FreenomDnsProvider>();
            return services;
        }
    }
}