using System;
using System.Threading.Tasks;

namespace DotNetCertBot.Domain
{
    public interface ICloudFlareService: IDisposable
    {
        Task<bool> CheckAuth();

        Task<bool> Login(string login, string password);

        Task AddChallenge(DnsChallenge challenge, string zoneName);

        Task ClearChallenge(string zoneName, string id);
    }
}