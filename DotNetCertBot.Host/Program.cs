using System.Threading.Tasks;
using DotNetCertBot.CloudFlareUserApi;
using DotNetCertBot.LetsEncrypt;
using DotNetCertBot.NoOp;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = ApplicationExtensions.ConfigureApp(services =>
            {
                services.AddSingleton<CertificateService>();
                services.AddSingleton(args.CreateConfiguration());
                services.AddCloudFlare();
                services.AddLetsEncrypt();
            });

            using var scope = provider.CreateScope();
            var certificateService = scope.ServiceProvider.GetService<CertificateService>();
            await certificateService.Issue();
        }
    }
}