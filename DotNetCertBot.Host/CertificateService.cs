using System;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Configuration;

namespace DotNetCertBot.Host
{
    public class CertificateService
    {
        private readonly IAcmeService _acme;
        private readonly ICloudFlareService _cloudFlareService;
        private readonly IConfiguration _configuration;

        public CertificateService(IAcmeService acme, ICloudFlareService cloudFlareService, IConfiguration configuration)
        {
            _acme = acme;
            _cloudFlareService = cloudFlareService;
            _configuration = configuration;
        }

        public async Task Issue()
        {
            await _acme.Login(_configuration.GetEmail()); var isAuth = await _cloudFlareService.CheckAuth();
            if(!isAuth)
                if(!await _cloudFlareService.Login(_configuration.GetEmail(), _configuration.GetPassword()))
                    throw new Exception("Can't authorize in cloudflare");

            var order = await _acme.CreateOrder(_configuration.GetDomain());
            var challenge = await _acme.ChallengeDNS(order);

            await _cloudFlareService.AddChallenge(challenge, _configuration.GetZone());
            try
            {
                await _acme.Validate(challenge);
                var cert = await _acme.GetCertificate(order);
                await cert.WriteToFile(_configuration.GetOutput());
            }
            finally
            {
                await _cloudFlareService.ClearChallenge(_configuration.GetZone(), challenge.Name);
            }
            _cloudFlareService.Dispose();
        }
    }
}