using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skidbladnir.Modules;

namespace DotNetCertBot.Host
{
    public class StartupModule : RunnableModule
    {
        public static IList<Type> DynamicDependsModules = new List<Type>();

        public override Type[] DependsModules => DynamicDependsModules.ToArray();

        public override void Configure(IServiceCollection services)
        {
            var certbotConfiguration = Configuration.AppConfiguration.GetConfiguration();
            services.AddSingleton(certbotConfiguration);
            services.AddSingleton(Configuration.AppConfiguration);
            services.AddSingleton<CertificateService>();
        }

        public override Task StartAsync(IServiceProvider provider, CancellationToken cancellationToken)
        {
            var certificateService = provider.GetService<CertificateService>();
            var logger = provider.GetService<ILogger<StartupModule>>();
            logger.LogInformation("Begin certificate issue process");
            return certificateService.Issue();
        }
    }
}