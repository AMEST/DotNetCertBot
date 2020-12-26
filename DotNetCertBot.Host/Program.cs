using System.Threading.Tasks;
using DotNetCertBot.CloudFlareUserApi;
using DotNetCertBot.LetsEncrypt;
using DotNetCertBot.NoOp;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCertBot.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var provider = ApplicationExtensions.ConfigureApp(services =>
            {
                var configuration = args.CreateConfiguration();
                services.AddSingleton<CertificateService>();
                services.AddSingleton(configuration);
                switch (configuration.GetNoOp())
                {
                    case ConfigurationExtensions.NoOpMode.Full:
                        services.AddNoOpAcme();
                        services.AddNoOpCloudFlare();
                        break;
                    case ConfigurationExtensions.NoOpMode.Acme:
                        services.AddNoOpAcme();
                        services.AddCloudFlare();
                        break;
                    case ConfigurationExtensions.NoOpMode.None:
                    default:
                        services.AddLetsEncrypt();
                        services.AddCloudFlare();
                        break;
                }
            });

            using var scope = provider.CreateScope();
            var certificateService = scope.ServiceProvider.GetService<CertificateService>();
            await certificateService.Issue();
        }
    }
}