using System;
using System.IO;
using System.Threading.Tasks;
using DotNetCertBot.Domain;

namespace DotNetCertBot.Host
{
    public static class ApplicationExtensions
    {
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