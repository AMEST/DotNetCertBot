****![Certbot Build](https://github.com/AMEST/DotNetCertBot/workflows/Certbot%20Build/badge.svg)
![hub.docker.com](https://img.shields.io/docker/pulls/eluki/freenom-cloudflare-certbot.svg)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/amest/DotNetCertBot)
![GitHub](https://img.shields.io/github/license/amest/DotNetCertBot)

# DotNetCertBot with cloudflare (and freenom) DNS challenge for [Freenom (tk/ml)](https://freenom.com) domains

- [DotNetCertBot with cloudflare (and freenom) DNS challenge for Freenom (tk/ml) domains](#dotnetcertbot-with-cloudflare-and-freenom-dns-challenge-for-freenom-tkml-domains)
  - [Links](#links)
  - [Description](#description)
  - [Download](#download)
  - [How to use](#how-to-use)
    - [Available providers](#available-providers)
    - [CommandLine arguments:](#commandline-arguments)
      - [Windows cmd or Linux bash](#windows-cmd-or-linux-bash)
      - [Docker container (linux)](#docker-container-linux)
  - [How to build](#how-to-build)
      - [Build binaries](#build-binaries)
      - [Build docker container](#build-docker-container)

## Links

* [Docker Hub](https://hub.docker.com/r/eluki/freenom-cloudflare-certbot)

## Description

The app was written in connection with CloudFlare's restrictions on using its api to manage DNS records .tk .ml .cf and other free domain names from Freenom.

Under the hood is a regular client up to Let's encrypt and the code for the selenium driver, where the application automatically, emulating the behavior of the login user in cloudflare, selects the desired zone, adds an entry for the DNS Challenge and after the request is validated by the certification authority, saves the certificate and deletes the entry from the DNS

Also added the ability to issue certificates for domains issued through Freenom and continue to use the standard dns provided by Freenom. To do this, you need to specify the required provider: `--provider freenom`

## Download
1. Shell:
   1. [Windows x86 binaries](https://github.com/AMEST/DotNetCertBot/releases/latest/download/CertBot.Cli-win-x86.zip)
   2. [Linux x64 binaries](https://github.com/AMEST/DotNetCertBot/releases/latest/download/CertBot.Cli-linux-x64.zip)
2. [Docker container](https://hub.docker.com/r/eluki/freenom-cloudflare-certbot)

## How to use

### Available providers

Available DNS providers for acme dns challenge:
1. Cloudflare - Used headless chrome, for issue certificate for free freenom domains. Also suitable for another domains who use cloudflare dns
1. Freenom - suitable for issuing certificates for domains that have been registered through Freenom or using freenom dns

### CommandLine arguments:
| Argument   | Description                                                                                                                               |
| ---------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| -e         | Required. Email for cloudflare (and it use for let's enctypt)                                                                             |
| -p         | Required. Password for cloudflare account                                                                                                 |
| -z         | Required. Zone name in cloudflare (main domain name)                                                                                      |
| -d         | Required. Domain name for which the certificate is issued (is a subdomain of the zone)                                                    |
| --provider | (Default: cloudflare) DNS provider through which the dns record will be added for validation through ACME. Providers: Cloudflare, freenom |
| -h         | (Default: true) Selenium driver headless mode                                                                                             |
| -o         | (Default: app directory) Directory where saved generated certificates                                                                     |
| -t         | (Default: pem) Certificates output type. Pem - two pem files with private key and cert; Acme - certificate in acme.json format.           |
| --noop     | (Default: None) Noop mode start half functional or test mode for tesing sctipts or schedules. NoOp modes (full,acme, none)                |

#### Windows cmd or Linux bash
For issue certificate in shell (not in container), on pc **should be installed chrome 87.xx version**. **In prepared assemblies for windows and linux, chromedriver is already included.** If chrome installed and app downloaded, you can run next command for automatic issue certificate.  
*Windows:*

```cmd
DotNetCertBot.Host.exe -e example@gmail.com -p VerySecretCloudflarePass -z example.tk -d subdomain.example.tk
```

*Linux:*

```bash
./DotNetCertBot.Host -e example@gmail.com -p VerySecretCloudflarePass -z example.tk -d subdomain.example.tk
```

When success issue certificate, in `DotNetCertBot.Host` app folder will appear two files:

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

## How to build

#### Build binaries

Two scripts are prepared for the build, after running which, the compiled application with all dependencies, including chromedriver, will appear in the published folder.
Scripts:
1. `Build-linux.sh` - start build application for linux-x64
2. `Build-winx86.bat` - start build application for win-x86   

#### Build docker container

```
docker build -t certbot .
```
