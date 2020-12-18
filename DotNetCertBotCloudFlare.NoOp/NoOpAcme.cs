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
            return Task.FromResult(new ChallengeOrder());
        }

        public Task<DnsChallenge> ChallengeDNS(ChallengeOrder order)
        {
            return Task.FromResult(new DnsChallenge{Name = "_acme-challenge.monitoring",Value = "1234"});
        }

        public Task Validate(DnsChallenge challenge)
        {
            return Task.CompletedTask;
        }

        public Task<CertificateResult> GetCertificate(ChallengeOrder order)
        {
            return Task.FromResult(new CertificateResult());
        }
    }
}