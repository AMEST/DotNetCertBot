namespace DotNetCertBot.Domain
{
    public class AcmeCertificate
    {
        public AcmeCertificateDomain Domain { get; set; } = new AcmeCertificateDomain();
        public string Certificate { get; set; }
        public string Key { get; set; }

        public class AcmeCertificateDomain
        {
            public string Main { get; set; }
            public string[] SANs { get; set; }
        }
    }
}