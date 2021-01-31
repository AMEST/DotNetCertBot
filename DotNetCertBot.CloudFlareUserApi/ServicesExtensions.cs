using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.CloudFlareUserApi
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCloudFlareDnsProvider(this IServiceCollection services)
        {
            services.AddSingleton<IDnsProviderService, CloudFlareServiceSelenium>();
            return services;
        }
    }
}