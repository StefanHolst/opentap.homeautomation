using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTap.HomeAutomation.Scheduling
{
    [AllowAnyChild]
    [Display("Every day at {DescriptionString}")]
    public class EveryDayStep : TestStep, ITimeTriggeredStep
    {
        public string Times { get; set; } = "12:00";

        public string DescriptionString => $"{string.Join(" and at ", ParseTimes(Times))}.";

        static TimeSpan[] ParseTimes(string timesString)
        {
            bool fault = false;
            var tss = timesString.Split(';').Select(x =>
            {
                if (TimeSpan.TryParse(x, out var ts))
                {
                    return ts;
                }
                fault = true;

                return TimeSpan.Zero;
            }).ToArray();
            if (fault) return null;
            return tss;
        }

        public EveryDayStep()
        {
            Rules.Add(() => ParseTimes(Times) != null, () => "times must be in a valid ';' time-span format. e.g 12:00", nameof(Times));
        }
        public override void Run()
        {
            RunChildSteps();
        }

        public TimeSpan TimeToTrigger
        {
            get
            {
                var now = DateTime.Now;
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                List<TimeSpan> delays = new List<TimeSpan>();
                foreach (var time in ParseTimes(Times))
                {
                    var slot = today + time;
                    if (slot < now)
                    {
                        slot = tomorrow + time;
                    }
                    delays.Add((slot - now));
                }
                return delays.Min();
            }
        }
    }
}