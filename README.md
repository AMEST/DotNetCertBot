# DotNetCertBot with cloudflare DNS challenge for [Freenom](https://freenom.com) domains

- [DotNetCertBot with cloudflare DNS challenge for Freenom domains](#dotnetcertbot-with-cloudflare-dns-challenge-for-freenom-domains)
  - [Description](#description)
  - [How to use](#how-to-use)

## Description
The app was written in connection with CloudFlare's restrictions on using its api to manage DNS records .tk .ml .cf and other free domain names from Freenom.

Under the hood is a regular client up to Let's encrypt and the code for the selenium driver, where the application automatically, emulating the behavior of the login user in cloudflare, selects the desired zone, adds an entry for the DNS Challenge and after the request is validated by the certification authority, saves the certificate and deletes the entry from the DNS

## How to use
**CommandLine arguments**:
| Argument |                                      Description                                       |
| -------- | -------------------------------------------------------------------------------------- |
| -e       | Required. Email for cloudflare (and it use for let's enctypt)                          |
| -p       | Required. Password for cloudflare account                                              |
| -z       | Required. Zone name in cloudflare (main domain name)                                   |
| -d       | Required. Domain name for which the certificate is issued (is a subdomain of the zone) |
| -h       | (Default: true) Selenium driver headless mode                                          |

**Example (windows cmd):**
```cmd
DotNetCertBot.Host.exe -e example@gmail.com -p VerySecretCloudflarePass -z example.tk -d subdomain.example.tk
```