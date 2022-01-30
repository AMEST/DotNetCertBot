using System.Globalization;

namespace DotNetCertBot.Domain
{
    public class AcmeAccount
    {
        public string Email { get; set; }

        public AcmeAccountRegistration Registration { get; set; } = new AcmeAccountRegistration();

        public string PrivateKey { get; set; }

        public string KeyType { get; set; } = "2048";

    }

    public class AcmeAccountRegistration
    {

        public RegistrationBody Body { get; set; } = new RegistrationBody();

        public string Uri { get; set; }

        public class RegistrationBody
        {
            public string Status { get; set; } = "valid";
            public string[] Contact { get; set; }
        }
    }
}