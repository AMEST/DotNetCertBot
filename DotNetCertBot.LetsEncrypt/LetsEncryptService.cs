using System;
using System.Linq;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Logging;

namespace DotNetCertBot.LetsEncrypt
{
    public class LetsEncryptService : IAcmeService
    {
        private readonly ILogger<LetsEncryptService> _logger;
        private IAcmeContext _acme = new AcmeContext(WellKnownServers.LetsEncryptV2, badNonceRetryCount: 5);
        private string _accountEmail;
        public LetsEncryptService(ILogger<LetsEncryptService> logger)
        {
            _logger = logger;
        }

        public async Task Login(string email)
        {
            await _acme.NewAccount(email, true);
            _accountEmail = email;
        }

        public async Task<ChallengeOrder> CreateOrder(string domain)
        {
            _logger.LogInformation("New certificate order for {domain}", domain);
            var order = await _acme.NewOrder(new[] {domain});
            return new ChallengeOrder
            {
                Order = order,
                DnsName = domain
            };
        }

        public async Task<DnsChallenge> ChallengeDNS(ChallengeOrder challengeOrder)
        {
            var order = (IOrderContext) challengeOrder.Order;
            var authz = (await order.Authorizations()).First();
            var dnsChallenge = await authz.Dns();
            var dnsTxt = _acme.AccountKey.DnsTxt(dnsChallenge.Token);
            return new DnsChallenge
            {
                Name = $"_acme-challenge.{challengeOrder.DnsName}",
                Value = dnsTxt,
                Context = dnsChallenge
            };
        }

        public async Task Validate(DnsChallenge challenge)
        {
            var challengeConext = (IChallengeContext) challenge.Context;

            _logger.LogInformation("Wait 2 minutes while dns records apply and let's encrypt can validate challenge");
            await Task.Delay(TimeSpan.FromMinutes(2));

            var challengeValidation = await challengeConext.Validate();
            _logger.LogInformation("Validation status:{status}", challengeValidation?.Status);

            var validateCount = 1;
            while (challengeValidation?.Status != ChallengeStatus.Valid && validateCount++ < 30)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));

                challengeValidation = await challengeConext.Resource();

                if (!string.IsNullOrEmpty(challengeValidation?.Error?.Detail))
                    _logger.LogWarning("Error detail:{errorsDetail}\nError statusCode:{errorsStatusCode}",
                        challengeValidation.Error?.Detail, challengeValidation.Error?.Status);

                _logger.LogInformation("Validation status:{status}", challengeValidation?.Status);
            }
        }

        public async Task<CertificateResult> GetCertificate(ChallengeOrder challengeOrder)
        {
            var order = (IOrderContext) challengeOrder.Order;
            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
            CertificateChain cert;
            try
            {
                cert = await TryOrderGenerate(order, challengeOrder.DnsName, privateKey);
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                cert = await TryOrderGenerate(order, challengeOrder.DnsName, privateKey);
            }
            
            return new CertificateResult
            {
                Cert = cert.ToPem(),
                Key = privateKey.ToPem(),
                Domain = challengeOrder.DnsName.Replace("*", "wildcard")
            };
        }

        private Task<CertificateChain> TryOrderGenerate(IOrderContext order, string dnsName, IKey privateKey)
        {
            return order.Generate(new CsrInfo
            {
                CountryName = "RU",
                State = "Svr",
                Locality = "Yekaterinburg",
                Organization = "AutomaticCA",
                OrganizationUnit = "Dev",
                CommonName = dnsName,
            }, privateKey);
        }

        public async Task<Domain.AcmeAccount> GetAccount()
        {
            var acmeAccount = await _acme.Account();
            var account = new Domain.AcmeAccount
            {
                PrivateKey = _acme.AccountKey.ToPem(),
                Email = _accountEmail
            };
            account.Registration.Uri = acmeAccount.Location.ToString();
            account.Registration.Body.Contact = new string[]{$"mailto:{_accountEmail}"};
            
            return account;
        }
    }
}