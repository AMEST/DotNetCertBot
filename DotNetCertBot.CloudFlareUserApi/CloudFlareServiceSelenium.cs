using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCertBot.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using Selenium.Extensions;
using Selenium.WebDriver.UndetectedChromeDriver;

namespace DotNetCertBot.CloudFlareUserApi
{
    public class CloudFlareServiceSelenium : IDnsProviderService
    {
        private readonly ILogger<CloudFlareServiceSelenium> _logger;
        private readonly SlDriver _driver;
        private readonly IWebDriver _secondTab;
        private readonly WebDriverWait _waiter;
        private const string CloudFlareLoginUrl = "https://dash.cloudflare.com/login?lang=en-US";

        public CloudFlareServiceSelenium(IConfiguration configuration, ILogger<CloudFlareServiceSelenium> logger)
        {
            _logger = logger;
            var userAgent = RandomUserAgent.Generate();
            _logger.LogInformation($"Initialize Chrome driver with userAgent: {userAgent}");
            var chromeOptions = new ChromeOptions();
            if (configuration.IsHeadless())
            {
                chromeOptions.AddArguments("headless");
                _logger.LogInformation("Enable Headless mode");
            }

            chromeOptions.AddExcludedArgument("enable-automation");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--lang=en-US");
            _driver = UndetectedChromeDriver.Instance("test", chromeOptions);
            _driver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            // _driver.ExecuteCustomDriverCommand("Network.setUserAgentOverride", new Dictionary<string, object>
            // {
            //     {"userAgent", userAgent}
            // });
            _driver.ExecuteScript("window.open()");
            _secondTab = _driver.SwitchTo().NewWindow(WindowType.Tab);
            _secondTab.Navigate().GoToUrl(CloudFlareLoginUrl);
            _waiter = new WebDriverWait(_secondTab, TimeSpan.FromMinutes(3));
        }

        public async Task<bool> CheckAuth()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            var spanList = _secondTab.FindElements(By.TagName("span"));
            return await Task.Run(() =>
                !spanList.Any(span => span.Text.Equals("Log in to Cloudflare", StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> Login(string login, string password)
        {
            _logger.LogInformation("Login into cloudflare with email {email}", login);
            var loginInput = _secondTab.FindElement(By.Name("email"));
            var passwordInput = _secondTab.FindElement(By.Name("password"));
            await MouseClick(loginInput);
            loginInput.Clear();
            loginInput.SendKeys(login);
            await MouseClick(passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(password);
            await Task.Delay(200);
            var captcha = _secondTab.FindElement(By.TagName("iframe"));
            if(captcha != null)
            {
                var frame = _secondTab.SwitchTo().Frame(captcha);
                await MouseClick(frame.FindElement(By.TagName("input")));
            }
            await Task.Delay(200);
            passwordInput.Submit();
            await Task.Delay(TimeSpan.FromSeconds(3));
            return await CheckAuth();
        }

        public async Task AddChallenge(DnsChallenge challenge, string zoneName)
        {
            _logger.LogInformation("Add txt record '{txtName} to zone {zoneName}'", challenge.Name, zoneName);
            await Task.Delay(TimeSpan.FromSeconds(10));
            await GoToZoneDns(zoneName);
            await Task.Delay(TimeSpan.FromSeconds(6));
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
            if (_secondTab != null)
            {
                var screenshot = _secondTab.TakeScreenshot();
                screenshot.SaveAsFile("last-view.png");
                _logger.LogInformation($"Last page url: {_secondTab.Url}");
            }

            _secondTab?.Close();
            _secondTab?.Dispose();
        }

        private async Task AddTxtRecord(string name, string value)
        {
            await Task.Run(async () =>
            {
                var addButton = _waiter.Until(d => d.FindElement(By.XPath("//span[contains(text(),'Add record')]")));
                await MouseClick(addButton);
                var addRecordForm = _waiter.Until(d => d.FindElement(By.XPath("//div[@data-testid = 'dns-table-record-form']")));
                var dropDown = addRecordForm.FindElements(By.TagName("button")).First(b =>
                    b.GetAttribute("id").Contains("downshift") && b.GetAttribute("id").Contains("toggle-button"));
                await MouseClick(dropDown);
                var txtOption = _waiter.Until(d => d.FindElement(By.XPath("//li[contains(text(),'TXT')]")));
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
            var action = new Actions(_secondTab);
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