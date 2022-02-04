using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTap.HomeAutomation
{
    /// <summary>
    /// This test step is able to schedule the run of other test steps.
    /// </summary>
    [AllowAnyChild]
    public class ScheduleStep : TestStep
    {
        public override void Run()
        {
            while (TapThread.Current.AbortToken.IsCancellationRequested == false)
            {
                var nextup = ChildTestSteps.OfType<IScheduledStep>()
                    .OrderBy(x => x.DelayFromNow)
                    .FirstOrDefault();
                if (nextup == null)
                    break;
                var wait = nextup.DelayFromNow;
                Log.Info("Running {0} at {1}s", nextup.GetFormattedName(), wait);
                
                TapThread.Sleep(wait);
                RunChildStep(nextup);
            }
        }
    }

    [AllowAnyChild]
    [Display("Every day at {DescriptionString}")]
    public class EveryDayStep : TestStep, IScheduledStep
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

        public TimeSpan DelayFromNow
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
    
    [AllowAnyChild]
    [Display("Repeat Every {Every}")]
    public class TimeIntervalStep : TestStep, IScheduledStep
    {
        public TimeSpan Every { get; set; } = TimeSpan.FromSeconds(5);
        private DateTime lastRun;
        public override void Run()
        {
            lastRun = DateTime.Now;
            RunChildSteps();
        }

        public TimeSpan DelayFromNow {
            get
            {
                var nextTime = (lastRun + Every) - DateTime.Now;
                if (nextTime < TimeSpan.Zero)
                {
                    return TimeSpan.Zero;
                }

                return nextTime;
            }
            
        }
    }

    public interface IScheduledStep : ITestStep
    {
        TimeSpan DelayFromNow { get; }
    }
}