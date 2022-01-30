namespace DotNetCertBot.Domain
{
    public class AcmeCertificateResult
    {
        public AcmeAccount Account { get; set; }

        public AcmeCertificate[] Certificates { get; set; }

        public static AcmeCertificateResult Create(AcmeAccount account, CertificateResult certificate)
        {
            var acmeCertificate = new AcmeCertificate();
            acmeCertificate.Certificate = certificate.Cert;
            acmeCertificate
        }
    }
}