using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;
using Skidbladnir.Modules;

namespace DotNetCertBot.CloudFlareUserApi
{
    public class CloudFlareDnsProviderModule : Module
    {
        public override void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDnsProviderService, CloudFlareServiceSelenium>();
        }
    }
}