using System;
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
            IScheduledStep lastStep = null;
            while (TapThread.Current.AbortToken.IsCancellationRequested == false)
            {
                var nextup = ChildTestSteps.OfType<IScheduledStep>()
                    .OrderBy(x => x.DelayFromNow)
                    .FirstOrDefault();
                if (nextup == null)
                    break;
                var wait = nextup.DelayFromNow;
                if (lastStep == nextup && wait <= TimeSpan.FromSeconds(0.01))
                {
                    TapThread.Sleep(TimeSpan.FromSeconds(0.01));
                    continue;
                }
                Log.Info("Running {0} at {1}s", nextup.GetFormattedName(), wait);
                
                TapThread.Sleep(wait);
                TapThread.Start(() => RunChildStep(nextup));
                lastStep = nextup;
            }
        }
    }
}