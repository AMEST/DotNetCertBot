using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;
using Skidbladnir.Modules;

namespace DotNetCertBot.FreenomDnsProvider
{
    public class FreenomDnsProviderModule : Module
    {
        public override void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDnsProviderService, FreenomDnsProvider>();
        }
    }
}