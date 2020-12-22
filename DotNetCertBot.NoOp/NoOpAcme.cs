using System;
using System.Threading.Tasks;
using DotNetCertBot.Domain;

namespace DotNetCertBot.NoOp
{
    public class NoOpAcme: IAcmeService
    {
        public Task Login(string email)
        {
            return Task.CompletedTask;
        }

        public Task<ChallengeOrder> CreateOrder(string domain)
        {
            return Task.FromResult(new ChallengeOrder{DnsName = domain});
        }

        public Task<DnsChallenge> ChallengeDNS(ChallengeOrder order)
        {
            return Task.FromResult(new DnsChallenge{Name = "_acme-challenge.bot",Value = "1234"});
        }

        public Task Validate(DnsChallenge challenge)
        {
            return Task.Delay(TimeSpan.FromSeconds(15));
        }

        public Task<CertificateResult> GetCertificate(ChallengeOrder order)
        {
            return Task.FromResult(new CertificateResult
            {
                Cert = "certificate pem content. NOOP",
                Key = "certificate key content. NOOP",
                Domain = order.DnsName
            });
        }
    }
}