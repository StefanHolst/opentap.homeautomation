using System;
using System.Collections.Generic;
using LifxNet;
using OpenTap;

namespace opentap_homeautomation
{
    public class LifxDevice : Dut
    {
        
    }

    public class LifxSettings : ComponentSettings<LifxSettings>
    {
        private LifxClient client;
        
        public Action ScanForDevices => async () =>
        {
            var client = await LifxNet.LifxClient.CreateAsync();
            client.DeviceDiscovered += Client_DeviceDiscovered;
            // client.DeviceLost += Client_DeviceLost;
            client.StartDeviceDiscovery();
        };
        
        private async void Client_DeviceDiscovered(object sender, LifxNet.LifxClient.DeviceDiscoveryEventArgs e)
        {
            var bulb = e.Device as LifxNet.LightBulb;
            
            
            await client.SetDevicePowerStateAsync(bulb, true); //Turn bulb on
            await client.SetColorAsync(bulb, Colors.Red, 2700); //Set color to Red and 2700K Temperature			
        }

        public List<LifxDevice> Devices { get; set; } = new List<LifxDevice>();
    }
}