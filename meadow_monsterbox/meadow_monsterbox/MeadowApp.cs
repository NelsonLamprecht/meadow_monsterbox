using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;

using meadow_monsterbox.Controllers;

namespace meadow_monsterbox
{
    public class MeadowApp : App<F7FeatherV1, MeadowApp>
    {
        private MapleServer _mapleServer;

        private const string appConfigFileName = "app.config.json";

        public MeadowApp()
        {
            try
            {
                Device.Information.DeviceName = "MeadowF7v1-1";
                Initialize().Wait();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task Initialize()
        {
            Console.WriteLine("Initializing hardware...");
            LedController.Current.Initialize();
            RelayController.Current.Initialize();
            AppConfigRoot appConfigRoot = await GetAppConfig();

            if (appConfigRoot != null
                &&
                appConfigRoot.Network != null
                &&
                appConfigRoot.Network.Wifi != null
                &&
                appConfigRoot.Network.Wifi.SSID != null
                &&
                appConfigRoot.Network.Wifi.Password != null)
            {

                ConnectionResult connectionResult = await Device.WiFiAdapter.Connect(appConfigRoot.Network.Wifi.SSID, appConfigRoot.Network.Wifi.Password);
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }                
                _mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, true, RequestProcessMode.Serial, null);
                _mapleServer.Start();
                LedController.Current.SetColor(Color.Green);
            }
            else
            {
                throw new Exception("Unable to get network configuration from file.");
            }
        }

        private async Task<AppConfigRoot> GetAppConfig()
        {
            var appConfigFilePath = Path.Combine(MeadowOS.FileSystem.UserFileSystemRoot, appConfigFileName);
            var appConfig = await GetFileContentsAsync<AppConfigRoot>(appConfigFilePath);
            if (appConfig != default)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Network:");
                Console.WriteLine($"\tSSID: {appConfig.Network.Wifi.SSID}");
                Console.WriteLine($"\tPassword: {appConfig.Network.Wifi.Password}");
            }

            return appConfig;
        }

        private async Task<T> GetFileContentsAsync<T>(string path)
        {
            Console.WriteLine("GetFileContentsAsync()");
            Console.WriteLine($"\tFile: {Path.GetFullPath(path)} ");
            try
            {
                using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var fileContents = await streamReader.ReadToEndAsync();
                    var result = JsonSerializer.Deserialize<T>(fileContents);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default;
        }
    }
}