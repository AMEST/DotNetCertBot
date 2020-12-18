using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetCertBot.Host
{
    public static class ApplicationExtensions
    {
        public static IConfiguration CreateConfiguration(this string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-d", "domain" },
                { "-z", "zone" },
                { "-e", "email" },
                { "-p", "password" },
                { "-o", "output" },
                { "-h", "headless" }
            };
            var builder = new ConfigurationBuilder().AddCommandLine(args, switchMappings);
            return builder.Build();
        }

        public static IServiceProvider ConfigureApp(Action<IServiceCollection> buildAction)
        {
            var collection = new ServiceCollection();
            collection.AddLogging(b => b.AddConsole());
            buildAction?.Invoke(collection);
            return collection.BuildServiceProvider().CreateScope().ServiceProvider;
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

        public static async Task WriteToFile(this CertificateResult certificate, string outputPath = null)
        {
            var path = "";
            if (!string.IsNullOrWhiteSpace(outputPath))
                path = outputPath;
            //Write Cert
            using (var writer = File.CreateText(Path.Combine(path, $"{certificate.Domain}.pem")))
            {
                await writer.WriteLineAsync(certificate.Cert);
            }
            //Write Key
            using (var writer = File.CreateText(Path.Combine(path, $"{certificate.Domain}.key")))
            {
                await writer.WriteLineAsync(certificate.Key);
            }
        }
    }
}