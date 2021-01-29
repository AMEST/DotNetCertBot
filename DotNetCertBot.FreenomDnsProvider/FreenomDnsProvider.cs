using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Skidbladnir.Client.Freenom.Dns;

namespace DotNetCertBot.FreenomDnsProvider
{
    internal class FreenomDnsProvider : IDnsProviderService
    {
        private readonly IFreenomClient _client;

        public FreenomDnsProvider()
        {
            _client = FreenomClientFactory.Create();
        }

        public Task<bool> CheckAuth()
        {
            return _client.IsAuthenticated();
        }

        public async Task<bool> Login(string login, string password)
        {
            await _client.SignIn(login, password);
            return await _client.IsAuthenticated();
        }

        public async Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            var zones = await _client.GetZones();
            var neededZone = zones.FirstOrDefault(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            if(neededZone == null)
                throw new Exception($"Zone ({zoneName}) not found");

            var dnsRecord = new DnsRecord(challenge.Name,DnsRecordType.TXT)
            {
                Ttl = 600,
                Value = challenge.Value
            };
            await _client.AddDnsRecord(neededZone, dnsRecord);
        }

        public async Task ClearChallenge(string zoneName, string id)
        {
            var zones = await _client.GetZones();
            var neededZone = zones.FirstOrDefault(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase));
            if (neededZone == null)
                throw new Exception($"Zone ({zoneName}) not found");

            var records =  await _client.GetDnsRecords(neededZone);
            var neededRecord = records.FirstOrDefault(r =>
                r.Name.Equals(id, StringComparison.OrdinalIgnoreCase) && r.Type == DnsRecordType.TXT);

            if (neededRecord == null)
                throw new Exception($"Dns record ({id}) not found");

            await _client.RemoveDnsRecord(neededZone, neededRecord);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}