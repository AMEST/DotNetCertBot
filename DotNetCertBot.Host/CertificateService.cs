using DotNetCertBot.Domain;
using System;
using System.Threading.Tasks;

namespace DotNetCertBot.Host
{
    public class CertificateService
    {
        private readonly IAcmeService _acme;
        private readonly IDnsProviderService _dnsProviderService;
        private readonly CertBotConfiguration _configuration;

        public CertificateService(IAcmeService acme, IDnsProviderService dnsProviderService, CertBotConfiguration configuration)
        {
            _acme = acme;
            _dnsProviderService = dnsProviderService;
            _configuration = configuration;
        }

        public async Task Issue()
        {
            await _acme.Login(_configuration.Email); 
            var isAuth = await _dnsProviderService.CheckAuth();
            if(!isAuth)
                if(!await _dnsProviderService.Login(_configuration.Email, _configuration.Password))
                    throw new Exception("Can't authorize in dns provider");

            var order = await _acme.CreateOrder(_configuration.Domain);
            var challenge = await _acme.ChallengeDNS(order);

            await _dnsProviderService.AddChallenge(challenge, _configuration.Zone);
            try
            {
                await _acme.Validate(challenge);
                var cert = await _acme.GetCertificate(order);
                await cert.WriteToFile(_configuration.Output);
            }
            finally
            {
                await _dnsProviderService.ClearChallenge(_configuration.Zone, challenge.Name);
            }
            _dnsProviderService.Dispose();
        }
    }
}