using System;
using System.Threading.Tasks;
using DotNetCertBot.CloudFlareUserApi;
using DotNetCertBot.Domain;
using DotNetCertBot.FreenomDnsProvider;
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
                var certbotConfiguration = configuration.GetConfiguration();
                services.AddSingleton(certbotConfiguration);
                services.AddSingleton(configuration);
                services.AddSingleton<CertificateService>();
                switch (certbotConfiguration.NoOp)
                {
                    case NoOpMode.Full:
                        services.AddNoOpAcme();
                        services.AddNoOpDnsProvider();
                        break;
                    case NoOpMode.Acme:
                        services.AddNoOpAcme();
                        RegisterDnsProvider(services,certbotConfiguration);
                        break;
                    case NoOpMode.None:
                    default:
                        services.AddLetsEncrypt();
                        RegisterDnsProvider(services, certbotConfiguration);
                        break;
                }
            });

            using var scope = provider.CreateScope();
            var certificateService = scope.ServiceProvider.GetService<CertificateService>();
            await certificateService.Issue();
        }

        private static void RegisterDnsProvider(IServiceCollection services, CertBotConfiguration configuration)
        {
            switch (configuration.Provider)
            {
                case DnsProvider.Freenom:
                    services.AddFreenomDnsProvider();
                    break;
                case DnsProvider.CloudFlare:
                default:
                    services.AddCloudFlareDnsProvider();
                    break;
            }
        }
    }
}