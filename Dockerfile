FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY . /build
WORKDIR /build
	
RUN dotnet publish ./DotNetCertBot.Host/DotNetCertBot.Host.csproj -c Release --self-contained -r linux-x64 -p:PublishTrimmed=true -o /app; \
    dotnet nuget locals http-cache --clear;\
    dotnet nuget locals temp --clear

FROM selenium/standalone-chrome:87.0.4280.88 as certbot
COPY --from=build /app /certbot
WORKDIR /certbot
USER root
ENTRYPOINT ["./DotNetCertBot.Host"]
