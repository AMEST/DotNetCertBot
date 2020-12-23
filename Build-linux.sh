#!/bin/bash
dotnet clean --configuration Release
dotnet publish ./DotNetCertBot.Host/DotNetCertBot.Host.csproj -c Release --self-contained -r linux-x64 -o ./published
find . -type f -name "chromedriver*" -exec cp -n {} ./published/ \;