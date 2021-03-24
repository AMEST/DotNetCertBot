using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Logging;
using Skidbladnir.Client.Freenom.Dns;
using Skidbladnir.Utility.Common;

namespace DotNetCertBot.FreenomDnsProvider
{
    internal class FreenomDnsProvider : IDnsProviderService
    {
        private readonly ILogger<FreenomDnsProvider> _logger;
        private readonly IFreenomClient _client;
        private const int RetryCount = 5;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

        public FreenomDnsProvider(ILogger<FreenomDnsProvider> logger)
        {
            _logger = logger;
            _client = FreenomClientFactory.Create();
        }

        public Task<bool> CheckAuth()
        {
            return Retry.Do(async () => await _client.IsAuthenticated(), RetryCount, RetryDelay);
        }

        public async Task<bool> Login(string login, string password)
        {
            await Retry.Do(async () => await _client.SignIn(login, password), RetryCount, RetryDelay);
            return await Retry.Do(async () => await _client.IsAuthenticated(), RetryCount, RetryDelay);
        }

        public async Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            var zones = await Retry.Do(async () => await _client.GetZones(), RetryCount, RetryDelay);
            var neededZone = zones.FirstOrDefault(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            if (neededZone == null)
                throw new Exception($"Zone ({zoneName}) not found");

            var dnsRecord = new DnsRecord(NormalizeDnsName(challenge.Name, zoneName), DnsRecordType.TXT)
            {
                Ttl = 3600,
                Value = challenge.Value
            };
            await Retry.Do(async () => await _client.AddDnsRecord(neededZone, dnsRecord), RetryCount, RetryDelay);
            _logger.LogInformation(
                "{Date}: Wait 7 minutes because freenom name servers is so buggy. (Waiting for apply dns record)",
                DateTime.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(7));
        }

        public async Task ClearChallenge(string zoneName, string id)
        {
            var zones = await Retry.Do(async () => await _client.GetZones(), RetryCount, RetryDelay);
            var neededZone = zones.FirstOrDefault(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            if (neededZone == null)
                throw new Exception($"Zone ({zoneName}) not found");

            var records = await Retry.Do(async () => await _client.GetDnsRecords(neededZone), RetryCount, RetryDelay);
            var neededRecord = records.FirstOrDefault(r =>
                r.Name.Equals(NormalizeDnsName(id, zoneName), StringComparison.OrdinalIgnoreCase) &&
                r.Type == DnsRecordType.TXT);

            if (neededRecord == null)
                throw new Exception($"Dns record ({id}) not found");

            await Retry.Do(async () => await _client.RemoveDnsRecord(neededZone, neededRecord), RetryCount, RetryDelay);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private static string NormalizeDnsName(string dns, string zone)
        {
            var normalized = dns.Replace($".{zone}", "");
            if (normalized.EndsWith(".*"))
                normalized = normalized.Replace(".*", "");
            if (normalized.EndsWith("."))
                normalized = normalized.Substring(0, normalized.Length - 1);

            return normalized;
        }
    }
}