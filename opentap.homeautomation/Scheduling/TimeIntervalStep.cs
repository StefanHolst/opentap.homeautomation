using System;

namespace OpenTap.HomeAutomation.Scheduling
{
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
}