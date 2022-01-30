namespace DotNetCertBot.Domain
{
    public class CertBotConfiguration
    {
        public string Domain { get; set; }
        public string Zone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Output { get; set; }
        public OutputType OutputType { get; set; }
        public NoOpMode NoOp { get; set; } = NoOpMode.None;
        public DnsProvider Provider { get; set; } = DnsProvider.CloudFlare;
    }
}