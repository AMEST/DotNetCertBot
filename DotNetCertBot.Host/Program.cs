using System;
using System.Threading.Tasks;
using DotNetCertBot.CloudFlareUserApi;
using DotNetCertBot.Domain;
using DotNetCertBot.FreenomDnsProvider;
using DotNetCertBot.LetsEncrypt;
using DotNetCertBot.NoOp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skidbladnir.Modules;

namespace DotNetCertBot.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var collection = new ServiceCollection();
            collection.AddLogging(b => b.AddConsole()
                .SetMinimumLevel(LogLevel.Information));

            var configuration = args.CreateConfiguration();
            ConfigureDependedModules(configuration);

            collection.AddSkidbladnirModules<StartupModule>(configuration: configuration);
            using var provider = collection.BuildServiceProvider();
            await provider.StartModules();
        }

        private static void ConfigureDependedModules(IConfiguration configuration)
        {
            var certbotConfiguration = configuration.GetConfiguration();
            switch (certbotConfiguration.NoOp)
            {
                case NoOpMode.Full:
                    StartupModule.DynamicDependsModules.Add(typeof(AcmeNoOpModules));
                    StartupModule.DynamicDependsModules.Add(typeof(DnsNoOpModule));
                    break;
                case NoOpMode.Acme:
                    StartupModule.DynamicDependsModules.Add(typeof(AcmeNoOpModules));
                    RegisterDnsProvider(certbotConfiguration);
                    break;
                case NoOpMode.None:
                default:
                    StartupModule.DynamicDependsModules.Add(typeof(LetsEncryptAuthorityModule));
                    RegisterDnsProvider(certbotConfiguration);
                    break;
            }
        }

        private static void RegisterDnsProvider(CertBotConfiguration configuration)
        {
            switch (configuration.Provider)
            {
                case DnsProvider.Freenom:
                    StartupModule.DynamicDependsModules.Add(typeof(FreenomDnsProviderModule));
                    break;
                case DnsProvider.CloudFlare:
                default:
                    StartupModule.DynamicDependsModules.Add(typeof(CloudFlareDnsProviderModule));
                    break;
            }
        }
    }
}