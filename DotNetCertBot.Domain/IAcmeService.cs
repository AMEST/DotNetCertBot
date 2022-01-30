using System.Threading.Tasks;

namespace DotNetCertBot.Domain
{
    public interface IAcmeService
    {
        Task Login(string email);

        Task<ChallengeOrder> CreateOrder(string domain);

        Task<DnsChallenge> ChallengeDNS(ChallengeOrder order);

        Task Validate(DnsChallenge challenge);

        Task<CertificateResult> GetCertificate(ChallengeOrder order);

        Task<AcmeAccount> GetAccount();
    }
}