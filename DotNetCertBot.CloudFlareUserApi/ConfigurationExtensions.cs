using Microsoft.Extensions.Configuration;

namespace DotNetCertBot.CloudFlareUserApi
{
    public static class ConfigurationExtensions
    {
        public static bool IsHeadless(this IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["headless"]))
                return true;
            return bool.Parse(configuration["headless"]);
        }
    }
}