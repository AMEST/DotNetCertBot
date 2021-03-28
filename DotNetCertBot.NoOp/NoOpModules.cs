using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;
using Skidbladnir.Modules;

namespace DotNetCertBot.NoOp
{
    public class AcmeNoOpModules : Module
    {
        public override void Configure(IServiceCollection services)
        {
            services.AddSingleton<IAcmeService, NoOpAcme>();
        }
    }

    public class DnsNoOpModule : Module
    {
        public override void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDnsProviderService, NoOpDnsProvider>();
        }
    }
}