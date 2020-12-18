using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.CloudFlareUserApi
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCloudFlare(this IServiceCollection services)
        {
            services.AddSingleton<ICloudFlareService, CloudFlareServiceSelenium>();
            return services;
        }
    }
}