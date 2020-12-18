using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCertBot.CloudFlareUserApi
{
    public static class RandomUserAgent
    {
        private static IList<string> _userAgents = new List<string>
        {
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.53 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19041",
            "Mozilla/5.0 (X11; Ubuntu; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36",
            "Mozilla/5.0 (Linux; arm_64; Android 10; POCOPHONE F1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.127 YaBrowser/20.9.4.99.00 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; Android 5.1; LYO-L21) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Mobile Safari/537.36",
            "Mozilla/5.0 (Android 7.1.1; Tablet; rv:81.0) Gecko/81.0 Firefox/81.0",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3447.5 Safari/537.36",
            "Mozilla/5.0 (Linux; Android 9; Mi A3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.136 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; Android 10; SAMSUNG SM-N960F) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 Chrome/71.0.3578.99 Mobile Safari/537.36"
        };
        public static string Generate()
        {
            return _userAgents.OrderBy(q => Guid.NewGuid()).First();
        }
    }
}