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
        public static IServiceProvider ConfigureApp(Action<IServiceCollection> buildAction)
        {
            var collection = new ServiceCollection();
            collection.AddLogging(b => b.AddConsole());
            buildAction?.Invoke(collection);
            return collection.BuildServiceProvider().CreateScope().ServiceProvider;
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