using System;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Logging;

namespace DotNetCertBot.NoOp
{
    public class NoOpAcme : IAcmeService
    {
        private readonly ILogger<NoOpAcme> _logger;
        private string _email;

        public NoOpAcme(ILogger<NoOpAcme> logger)
        {
            _logger = logger;
        }

        public Task Login(string email)
        {
            _logger.LogInformation($"NoOp Login as `{email}`");
            _email = email;
            return Task.CompletedTask;
        }

        public Task<ChallengeOrder> CreateOrder(string domain)
        {
            _logger.LogInformation($"NoOp Create order for `{domain}`");
            return Task.FromResult(new ChallengeOrder { DnsName = domain });
        }

        public Task<DnsChallenge> ChallengeDNS(ChallengeOrder order)
        {
            _logger.LogInformation($"NoOp challenge for`{order.DnsName}`");
            return Task.FromResult(new DnsChallenge { Name = "_acme-challenge.bot", Value = "1234" });
        }

        public Task Validate(DnsChallenge challenge)
        {
            _logger.LogInformation($"NoOp validate`{challenge.Name}:{challenge.Value}`");
            return Task.Delay(TimeSpan.FromSeconds(15));
        }

        public Task<CertificateResult> GetCertificate(ChallengeOrder order)
        {
            _logger.LogInformation("Noop get certificate");
            return Task.FromResult(new CertificateResult
            {
                Cert = "certificate pem content. NOOP",
                Key = "certificate key content. NOOP",
                Domain = order.DnsName
            });
        }

        public Task<AcmeAccount> GetAccount()
        {
            return Task.FromResult(new AcmeAccount{
                Email = _email,
                KeyType = string.Empty,
                PrivateKey = string.Empty
            });
        }
    }
}