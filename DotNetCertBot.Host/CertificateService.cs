using DotNetCertBot.Domain;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Skidbladnir.Utility.Common;

namespace DotNetCertBot.Host
{
    public class CertificateService
    {
        private readonly ILogger<CertificateService> _logger;
        private readonly IAcmeService _acme;
        private readonly IDnsProviderService _dnsProviderService;
        private readonly CertBotConfiguration _configuration;

        public CertificateService(ILogger<CertificateService> logger, IAcmeService acme,
            IDnsProviderService dnsProviderService, CertBotConfiguration configuration)
        {
            _logger = logger;
            _acme = acme;
            _dnsProviderService = dnsProviderService;
            _configuration = configuration;
        }

        public async Task Issue()
        {
            await _acme.Login(_configuration.Email);
            var isAuth = await _dnsProviderService.CheckAuth();
            _logger.LogInformation($"Is Authenticated: {isAuth}");
            if (!isAuth)
                if (!await _dnsProviderService.Login(_configuration.Email, _configuration.Password))
                    throw new Exception("Can't authorize in dns provider");
            _logger.LogInformation($"Is Authenticated: {isAuth}");

            var order = await _acme.CreateOrder(_configuration.Domain);
            var challenge = await _acme.ChallengeDNS(order);
            _logger.LogInformation("Adding TXT Record: {Name}\t{Value}", challenge.Name, challenge.Value);
            await _dnsProviderService.AddChallenge(challenge, _configuration.Zone);
            try
            {
                await _acme.Validate(challenge);
                var cert = await _acme.GetCertificate(order);
                await cert.WriteToFile(_configuration.Output);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in validating or certificate download process");
                throw;
            }
            finally
            {
                await _dnsProviderService.ClearChallenge(_configuration.Zone, challenge.Name);
            }
        }
    }
}