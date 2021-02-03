using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Logging;
using Skidbladnir.Client.Freenom.Dns;

namespace DotNetCertBot.FreenomDnsProvider
{
    internal class FreenomDnsProvider : IDnsProviderService
    {
        private readonly ILogger<FreenomDnsProvider> _logger;
        private readonly IFreenomClient _client;

        public FreenomDnsProvider(ILogger<FreenomDnsProvider> logger)
        {
            _logger = logger;
            _client = FreenomClientFactory.Create();
        }

        public Task<bool> CheckAuth()
        {
            return TaskUtils.RunWithRetry(async () => await _client.IsAuthenticated());
        }

        public async Task<bool> Login(string login, string password)
        {
            await TaskUtils.RunWithRetry(async () => await _client.SignIn(login, password));
            return await TaskUtils.RunWithRetry(async () => await _client.IsAuthenticated());
        }

        public async Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            var zones = await TaskUtils.RunWithRetry(async () => await _client.GetZones());
            var neededZone = zones.FirstOrDefault(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            if (neededZone == null)
                throw new Exception($"Zone ({zoneName}) not found");

            var dnsRecord = new DnsRecord(NormalizeDnsName(challenge.Name, zoneName), DnsRecordType.TXT)
            {
                Ttl = 3600,
                Value = challenge.Value
            };
            await TaskUtils.RunWithRetry(async () => await _client.AddDnsRecord(neededZone, dnsRecord));
            _logger.LogInformation(
                "{Date}: Wait 7 minutes because freenom name servers is so buggy. (Waiting for apply dns record)",
                DateTime.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(7));
        }

        public async Task ClearChallenge(string zoneName, string id)
        {
            var zones = await TaskUtils.RunWithRetry(async () => await _client.GetZones());
            var neededZone = zones.FirstOrDefault(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            if (neededZone == null)
                throw new Exception($"Zone ({zoneName}) not found");

            var records = await TaskUtils.RunWithRetry(async () => await _client.GetDnsRecords(neededZone));
            var neededRecord = records.FirstOrDefault(r =>
                r.Name.Equals(NormalizeDnsName(id, zoneName), StringComparison.OrdinalIgnoreCase) &&
                r.Type == DnsRecordType.TXT);

            if (neededRecord == null)
                throw new Exception($"Dns record ({id}) not found");

            await TaskUtils.RunWithRetry(async () => await _client.RemoveDnsRecord(neededZone, neededRecord));
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