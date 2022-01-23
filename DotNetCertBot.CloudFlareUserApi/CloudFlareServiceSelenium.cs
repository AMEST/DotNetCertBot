using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace DotNetCertBot.CloudFlareUserApi
{
    public class CloudFlareServiceSelenium : IDnsProviderService
    {
        private readonly ILogger<CloudFlareServiceSelenium> _logger;
        private readonly ChromeDriver _driver;
        private readonly WebDriverWait _waiter;
        private const string CloudFlareLoginUrl = "https://dash.cloudflare.com/login?lang=en-US";

        public CloudFlareServiceSelenium(IConfiguration configuration, ILogger<CloudFlareServiceSelenium> logger)
        {
            _logger = logger;
            _logger.LogInformation("Initialize Chrome driver");
            var chromeOptions = new ChromeOptions();
            if (configuration.IsHeadless())
            {
                chromeOptions.AddArguments("headless");
                _logger.LogInformation("Enable Headless mode");
            }

            chromeOptions.AddExcludedArgument("enable-automation");
            chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--lang=en-US");
            _driver = new ChromeDriver(chromeOptions);
            _driver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            _driver.ExecuteChromeCommand("Network.setUserAgentOverride", new Dictionary<string, object>
            {
                {"userAgent", RandomUserAgent.Generate()}
            });
            _driver.Navigate().GoToUrl(CloudFlareLoginUrl);
            _waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(40));
        }

        public async Task<bool> CheckAuth()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            var spanList = _driver.FindElementsByTagName("span");
            return await Task.Run(() =>
                !spanList.Any(span => span.Text.Equals("Log in to Cloudflare", StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> Login(string login, string password)
        {
            _logger.LogInformation("Login into cloudflare with email {email}", login);
            var loginInput = _driver.FindElementByName("email");
            var passwordInput = _driver.FindElementByName("password");
            loginInput.Clear();
            loginInput.SendKeys(login);
            passwordInput.Clear();
            passwordInput.SendKeys(password);

            var submit = _driver.FindElementsByTagName("button")
                .SingleOrDefault(b => b.GetAttribute("type") == "submit");
            await Task.Run(() => submit?.Click());
            await Task.Delay(TimeSpan.FromSeconds(3));
            return await CheckAuth();
        }

        public async Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            _logger.LogInformation("Add txt record '{txtName} to zone {zoneName}'", challenge.Name, zoneName);
            await Task.Delay(TimeSpan.FromSeconds(10));
            await GoToZoneDns(zoneName);
            await Task.Delay(TimeSpan.FromMilliseconds(640));
            await AddTxtRecord(NormalizeDnsName(challenge.Name, zoneName), challenge.Value);
        }

        public Task ClearChallenge(string zoneName, string name)
        {
            return Task.Run(async () =>
            {
                var normalizedDnsName = NormalizeDnsName(name, zoneName);
                _logger.LogInformation("Remove {txtName} from zone {zoneName}", normalizedDnsName, zoneName);
                var dnsRecordCell = _waiter.Until(d =>
                    d.FindElement(By.XPath($"//div[contains(text(),'{normalizedDnsName}')]")));
                var dnsRecordRow = dnsRecordCell.GetParent().GetParent();
                var editButton = dnsRecordRow.FindElements(By.TagName("button")).Last();
                await MouseClick(editButton);
                var deleteButton = _waiter.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Delete')]")));
                await MouseClick(deleteButton);
                var iSureDeleteButton =
                    _waiter.Until(d => d.FindElements(By.XPath("//span[text() = 'Delete']"))).SingleOrDefault();
                await MouseClick(iSureDeleteButton);
                await Task.Delay(TimeSpan.FromSeconds(5));
            });
        }

        public void Dispose()
        {
            _driver?.Close();
            _driver?.Dispose();
        }

        private async Task AddTxtRecord(string name, string value)
        {
            await Task.Run(async () =>
            {
                var addButton = _waiter.Until(d => d.FindElement(By.XPath("//span[contains(text(),'Add record')]")));
                await MouseClick(addButton);
                var dropDown = _waiter.Until(d => d.FindElement(By.XPath("//button[@id = 'downshift-0-toggle-button']")));
                await MouseClick(dropDown);
                var txtOption = _waiter.Until(d => d.FindElement(By.Id("downshift-0-item-18")));
                await MouseClick(txtOption);
                var nameInput = _waiter.Until(d => d.FindElement(By.Name("name")));
                nameInput.Clear();
                nameInput.SendKeys(name);
                var contentInput = _waiter.Until(d => d.FindElement(By.Name("content")));
                contentInput.Clear();
                contentInput.SendKeys(value);
                var saveButton = _waiter.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Save')]")));
                await MouseClick(saveButton);
            });
        }

        private async Task GoToZoneDns(string zone)
        {
            await Task.Run(async () =>
            {
                _waiter.Until(d => d.FindElement(By.XPath("//div[@data-testid = 'zone-cards']")));
                var neededZone = _waiter.Until(d => d.FindElement(By.XPath($"//a[@data-testid = 'zone-card-{zone}']")));
                await MouseClick(neededZone);
                var dnsButton = _waiter.Until(d => d.FindElement(By.XPath("//span[contains(text(),'DNS')]")));
                await MouseClick(dnsButton);
            });
        }

        private async Task MouseClick(IWebElement element)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            var action = new Actions(_driver);
            action.MoveByOffset(element.Location.X, element.Location.Y);
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            action.Click(element);
            element.Click();
        }

        private static string NormalizeDnsName(string dns, string zone)
        {
            var normalized = dns.Replace($".{zone}", "");
            if (normalized.EndsWith(".*"))
                normalized = normalized.Replace(".*", "");
            if (normalized.EndsWith("."))
                normalized = normalized.Substring(0, normalized.Length - 1);

            return normalized;
        }
    }
}