using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenTap.HomeAutomation.Scheduling
{
    [AllowAnyChild]
    public class SunScheduleStep : TestStep, ITimeTriggeredStep
    {
        public SunEventType Type { get; set; }
        
        [Browsable(true)]
        public string Sunrise => sunEvent?.Sunrise.ToString();
        [Browsable(true)]
        public string Sunset => sunEvent?.Sunset.ToString();
        
        public override void Run()
        {
            // throw new NotImplementedException();
        }

        public TimeSpan TimeToTrigger
        {
            get
            {
                GetSunEvent();
                switch (Type)
                {
                    case SunEventType.Sunrise:
                        return sunEvent.Sunrise - DateTime.UtcNow;
                    case SunEventType.Sunset:
                        return sunEvent.Sunset - DateTime.UtcNow;
                }

                return TimeSpan.MaxValue;
            }
        }

        private static SunEvent sunEvent;
        private static HttpClient client = new HttpClient();
        
        private void GetSunEvent()
        {
            if (sunEvent != null && sunEvent.Sunrise < DateTime.UtcNow && sunEvent.Sunset < DateTime.UtcNow)
                return;

            var json = client.GetStringAsync("https://api.sunrise-sunset.org/json?lat=40.741895&lng=-73.989308").Result;
            
            var settings = new JsonSerializerSettings();
            settings.DateFormatString = "hh:mm:ss tt";
            settings.Culture = CultureInfo.InvariantCulture;

            var jobj = JObject.Parse(json);
            var results = jobj["results"];
            
            sunEvent = results?.ToObject<SunEvent>();

            if (sunEvent.Sunset < DateTime.UtcNow)
                sunEvent.Sunset.AddDays(1);
            if (sunEvent.Sunrise < DateTime.UtcNow)
                sunEvent.Sunrise.AddDays(1);
        }

        [JsonObject(Title = "results")]
        public class SunEvent
        {
            [JsonProperty("sunrise")]
            public DateTime Sunrise { get; set; }
            [JsonProperty("sunset")]
            public DateTime Sunset { get; set; }
        }
        public enum SunEventType
        {
            Sunrise,
            Sunset
        }
    }
}