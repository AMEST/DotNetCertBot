using System.Threading.Tasks;
using DotNetCertBot.Domain;

namespace DotNetCertBot.NoOp
{
    public class NoOpDnsProvider:IDnsProviderService
    {
        public void Dispose()
        {
        }

        public Task<bool> CheckAuth()
        {
            return Task.FromResult(true);
        }

        public Task<bool> Login(string login, string password)
        {
            return Task.FromResult(true);
        }

        public Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            return Task.FromResult("");
        }

        public Task ClearChallenge(string zoneName, string name)
        {
            return Task.CompletedTask;
        }
    }
}