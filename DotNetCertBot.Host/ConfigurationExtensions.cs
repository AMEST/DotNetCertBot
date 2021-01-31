using System;
using System.Collections.Generic;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Configuration;

namespace DotNetCertBot.Host
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration CreateConfiguration(this string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                {"-d", "Domain"},
                {"-z", "Zone"},
                {"-e", "Email"},
                {"-p", "Password"},
                {"-o", "Output"},
                {"-h", "Headless"},
                {"--provider", "Provider"},
                {"--noop", "NoOp"}
            };
            var builder = new ConfigurationBuilder().AddCommandLine(args, switchMappings);
            return builder.Build();
        }

        public static CertBotConfiguration GetConfiguration(this IConfiguration configuration)
        {
            var config = new CertBotConfiguration();
            configuration.Bind(config);
            return config;
        }
    }
}