using System.Threading.Tasks;

using Meadow;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using Meadow.Logging;

namespace meadow_monsterbox.Services.MapleService
{
    internal class MapleService : BaseService
    {
        private readonly IMeadowDevice device;
        private readonly INetworkAdapter networkAdapter;
        private MapleServer mapleServer;

        public MapleService(Logger logger, IMeadowDevice device, INetworkAdapter networkAdapter) : base(logger)
        {
            this.device = device;
            this.networkAdapter = networkAdapter;
        }

        public override Task Run()
        {
            mapleServer = new MapleServer(
                networkAdapter.IpAddress,
                port: 5417,
                advertise: true,
                // Parallel so a /sound isn't queued behind a /shake that holds its
                // request open until the shake finishes; see AGENTS.md for why this is safe
                processMode: RequestProcessMode.Parallel)
            {
                AdvertiseIntervalMs = 5000, // every 5 seconds
                DeviceName = device.Information.DeviceName
            };

            mapleServer.Start();
            return base.Run();
        }
    }
}
