using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenTap;

namespace opentap.homeautomation.Lifx
{
    public class LifxSettings : ComponentSettings<LifxSettings>
    {
        [Display("Search for Devices")]
        [Browsable(true)]
        public void ScanForDevices()
        {
            var lights = LifxApi.GetLights();
            
            foreach (var light in lights)
            {
                if (Lights.Any(d => d.uuid == light.uuid) == false)
                {
                    var newDeviceRequest = new NewDeviceRequest()
                    {
                        Name = $"{light.label} {light.uuid}"
                    };
                    UserInput.Request(newDeviceRequest);
                    
                    if (newDeviceRequest.response == NewDeviceRequest.Response.Yes)
                        Lights.Add(light);
                }
            }
        }

        public List<LifxLight> Lights { get; set; } = new List<LifxLight>();
    }

    public class NewDeviceRequest
    {
        public enum Response
        {
            Yes,
            No
        }
        public string Name { get; set; }
        [Submit]
        public Response response { get; set; }
    }
}