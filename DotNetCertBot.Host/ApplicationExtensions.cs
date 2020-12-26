using System;
using System.IO;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
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
            var certFilePath = Path.Combine(path, $"{certificate.Domain}.pem");
            await using (var writer = File.CreateText(certFilePath))
            {
                await writer.WriteLineAsync(certificate.Cert);
                Console.WriteLine($"Certificate writed to {certFilePath}");
            }
            //Write Key
            var keyFilePath = Path.Combine(path, $"{certificate.Domain}.key");
            await using (var writer = File.CreateText(keyFilePath))
            {
                await writer.WriteLineAsync(certificate.Key);
                Console.WriteLine($"Key writed to {keyFilePath}");
            }
        }
    }
}