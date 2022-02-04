using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace opentap.homeautomation.Lifx
{
    public static class LifxApi
    {
        private const string TOKEN = "c3cbfa9a4b80742bdb88ac41d3598d99a964a08d237d26d5052141371651ed80";
        private static HttpClient client;

        static LifxApi()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TOKEN);
        }

        public static List<LifxLight> GetLights()
        {
            var data = client.GetStringAsync("https://api.lifx.com/v1/lights/all").Result;
            return JsonConvert.DeserializeObject<List<LifxLight>>(data);
        }

        public static void TurnOn(LifxLight light)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("power", "on"));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void TurnOff(LifxLight light)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("power", "off"));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void SetBrightness(LifxLight light, double brightness)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("brightness", brightness.ToString()));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void SetColor(LifxLight light, string color)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("color", $"#{color}"));
            var content = new FormUrlEncodedContent(form);
            client.PutAsync($"https://api.lifx.com/v1/lights/id:{light.id}/state", content).Wait(1000);
        }
        public static void SetTemperature(LifxLight light, int temperature)
        {
            var form = new List<KeyValuePair<string, string>>();
            form.Add(new KeyValuePair<string, string>("color", "kelvin:" + temperature));
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