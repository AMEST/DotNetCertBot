using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DotNetCertBot.Host
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration CreateConfiguration(this string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                {"-d", "domain"},
                {"-z", "zone"},
                {"-e", "email"},
                {"-p", "password"},
                {"-o", "output"},
                {"-h", "headless"},
                {"--noop", "noop"}
            };
            var builder = new ConfigurationBuilder().AddCommandLine(args, switchMappings);
            return builder.Build();
        }


        public static string GetEmail(this IConfiguration configuration)
        {
            return configuration["email"];
        }

        public static string GetPassword(this IConfiguration configuration)
        {
            return configuration["password"];
        }

        public static string GetDomain(this IConfiguration configuration)
        {
            return configuration["domain"];
        }

        public static string GetZone(this IConfiguration configuration)
        {
            return configuration["zone"];
        }

        public static string GetOutput(this IConfiguration configuration)
        {
            return configuration["output"];
        }

        public static NoOpMode GetNoOp(this IConfiguration configuration)
        {
            return Enum.TryParse(typeof(NoOpMode), configuration["noop"], true, out var noopMode)
                ? (NoOpMode) noopMode
                : NoOpMode.None;
        }

        public enum NoOpMode
        {
            None,
            Acme,
            Full
        }
    }
}