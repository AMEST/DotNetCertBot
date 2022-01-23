using OpenQA.Selenium;

namespace DotNetCertBot.CloudFlareUserApi
{
    internal static class SeleniumExtensions
    {
        public static IWebElement GetParent(this IWebElement node)
        {
            return node.FindElement(By.XPath(".."));
        }
    }
}