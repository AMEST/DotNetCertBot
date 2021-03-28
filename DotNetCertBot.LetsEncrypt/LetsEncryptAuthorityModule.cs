using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;
using Skidbladnir.Modules;

namespace DotNetCertBot.LetsEncrypt
{
    public class LetsEncryptAuthorityModule : Module
    {
        public override void Configure(IServiceCollection services)
        {
            services.AddSingleton<IAcmeService, LetsEncryptService>();
        }
    }
}