dotnet clean --configuration Release
dotnet publish ./DotNetCertBot.Host/DotNetCertBot.Host.csproj -c Release --self-contained -r win-x86 -p:PublishTrimmed=true -o ./published
dir /s /b | for /f %%i in ('find "chromedriver"') do copy "%%i" .\published