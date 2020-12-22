# DotNetCertBot with cloudflare DNS challenge for [Freenom](https://freenom.com) domains

- [DotNetCertBot with cloudflare DNS challenge for Freenom domains](#dotnetcertbot-with-cloudflare-dns-challenge-for-freenom-domains)
  - [Description](#description)
  - [How to use](#how-to-use)
    - [CommandLine arguments:](#commandline-arguments)
      - [Windows cmd](#windows-cmd)
      - [Docker container (linux)](#docker-container-linux)

## Description
The app was written in connection with CloudFlare's restrictions on using its api to manage DNS records .tk .ml .cf and other free domain names from Freenom.

Under the hood is a regular client up to Let's encrypt and the code for the selenium driver, where the application automatically, emulating the behavior of the login user in cloudflare, selects the desired zone, adds an entry for the DNS Challenge and after the request is validated by the certification authority, saves the certificate and deletes the entry from the DNS

## How to use
### CommandLine arguments:
| Argument |                                      Description                                       |
| -------- | -------------------------------------------------------------------------------------- |
| -e       | Required. Email for cloudflare (and it use for let's enctypt)                          |
| -p       | Required. Password for cloudflare account                                              |
| -z       | Required. Zone name in cloudflare (main domain name)                                   |
| -d       | Required. Domain name for which the certificate is issued (is a subdomain of the zone) |
| -h       | (Default: true) Selenium driver headless mode                                          |
| -o       | (Default: app directory) Directory where saved generated certificates                  |

#### Windows cmd
For issue certificate on windows (not in container), on pc should be installed chrome 87.xx version. If chrome installed and app downloaded, you can run next command for automatic issue certificate.
```cmd
DotNetCertBot.Host.exe -e example@gmail.com -p VerySecretCloudflarePass -z example.tk -d subdomain.example.tk
```
When success issue certificate, in `DotNetCertBot.Host.exe` app folder will appear two files:
1. `subdomain.example.tk.pem` - Full chain certificate file
2. `subdomain.example.tk.key` - Private Key

#### Docker container (linux)
To issue a certificate in a container, you need to mount the directory where the certificates will appear.   
Because chrome will run inside the container, the host must have at least 200 MB of free RAM.
For start container and issue certificate, run next command:
```bash
docker run -v /tmp/certbot:/certbot/certs \
           --rm \ 
           -it \
           eluki/freenom-cloudflare-certbot \
            -e example@gmail.com \
            -p VerySecretCloudflarePass \
            -z example.tk \
            -d subdomain.example.tk \
            -o certs
```