using DotNetCertBot.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.LetsEncrypt
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddLetsEncrypt(this IServiceCollection services)
        {
            services.AddSingleton<IAcmeService, LetsEncryptService>();
            return services;
        }
    }
}