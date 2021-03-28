using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Logging;

namespace DotNetCertBot.NoOp
{
    public class NoOpDnsProvider : IDnsProviderService
    {
        private readonly ILogger<NoOpDnsProvider> _logger;

        public NoOpDnsProvider(ILogger<NoOpDnsProvider> logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
        }

        public Task<bool> CheckAuth()
        {
            _logger.LogInformation("NoOp check auth");
            return Task.FromResult(true);
        }

        public Task<bool> Login(string login, string password)
        {
            _logger.LogInformation($"NoOp login as `{login}`");
            return Task.FromResult(true);
        }

        public Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            _logger.LogInformation($"NoOp challenge `{challenge.Name}:{zoneName}`");
            return Task.FromResult("");
        }

        public Task ClearChallenge(string zoneName, string name)
        {
            _logger.LogInformation($"NoOp clear challenge`{zoneName}:{name}`");
            return Task.CompletedTask;
        }
    }
}