using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace OpenTap.HomeAutomation.Lifx
{
    public static class LifxApi
    {
        private static HttpClient client;

        static LifxApi()
        {
            client = new HttpClient();
            UpdateAuthorization("");
        }

        public static void UpdateAuthorization(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static List<LifxLight> GetLights()
        {
            var data = client.GetStringAsync("https://api.lifx.com/v1/lights/all").Result;
            return JsonConvert.DeserializeObject<List<LifxLight>>(data);
        }

        public static void TurnOn(LifxLight light, double duration = 1)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("power", "on"));
            form.Add(new KeyValuePair<string, string>("duration", string.Format("{0:0.0}", duration)));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void TurnOff(LifxLight light, double duration = 1)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("power", "off"));
            form.Add(new KeyValuePair<string, string>("duration", string.Format("{0:0.0}", duration)));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void SetBrightness(LifxLight light, double brightness, double duration = 1)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("brightness", brightness.ToString()));
            form.Add(new KeyValuePair<string, string>("duration", string.Format("{0:0.0}", duration)));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void SetColor(LifxLight light, string color, double duration = 1)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("color", $"#{color}"));
            form.Add(new KeyValuePair<string, string>("duration", string.Format("{0:0.0}", duration)));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void SetTemperature(LifxLight light, int temperature, double duration = 1)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("color", "kelvin:" + temperature));
            form.Add(new KeyValuePair<string, string>("duration", string.Format("{0:0.0}", duration)));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
    }

    public class LifxLight
    {
        public string id { get; set; }
        public string uuid { get; set; }
        public string label { get; set; }
        public bool connected { get; set; }

        public override string ToString()
        {
            return label;
        }
    }
}