namespace DotNetCertBot.Domain
{

    public class DnsChallenge
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public object Context { get; set; }
    }
}