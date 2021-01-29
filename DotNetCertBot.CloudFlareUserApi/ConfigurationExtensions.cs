using Microsoft.Extensions.Configuration;

namespace DotNetCertBot.CloudFlareUserApi
{
    public static class ConfigurationExtensions
    {
        public static bool IsHeadless(this IConfiguration configuration)
        {
            return string.IsNullOrEmpty(configuration["Headless"])
                   || bool.Parse(configuration["Headless"]);
        }
    }
}