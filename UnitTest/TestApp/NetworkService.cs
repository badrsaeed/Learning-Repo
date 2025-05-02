using System.Net.NetworkInformation;
using TestApp.Services;

namespace TestApp
{
    public class NetworkService
    {
        private readonly IDNSService _dnsService;

        public NetworkService(IDNSService dnsService)
        {
            _dnsService = dnsService;
        }

        public string GetNetworkName()
        {
            return "Badr";
        }

        public int Ping(string ip, int port)
        {
            if (ip.Length > 3 && port > 0)
                return 1;
            return -1;
        }

        public int CalculateTimeOut(int a, int p)
        {
            return a + p;
        }

        public DateTime GetLastTestDateTime()
        {
            return DateTime.Now;
        }

        public PingOptions GetPingOptions()
        {
            return new PingOptions
            {
                DontFragment = true,
                Ttl = 1
            };
        }

        public IEnumerable<int> GetAllPingTryies()
        {
            return Enumerable.Range(0, 100);
        }

        public IEnumerable<PingOptions> GetPingOptionsList()
        {
            return new List<PingOptions>
            {
                new PingOptions { DontFragment = true, Ttl = 1 },
                new PingOptions { DontFragment = false, Ttl = 1 },
                new PingOptions { DontFragment = true, Ttl = 2 },
                new PingOptions { DontFragment = false, Ttl = 2 },
            };
        }

        public string SendTestPing()
        {
            var isSuccess = _dnsService.SendPing();

            if (isSuccess)
                return "Successfull Send";

            return "Faild Send";
        }
    }
}
