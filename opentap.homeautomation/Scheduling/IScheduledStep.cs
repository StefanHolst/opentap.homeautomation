using System;

namespace OpenTap.HomeAutomation.Scheduling
{
    public interface IScheduledStep : ITestStep
    {
        TimeSpan DelayFromNow { get; }
    }
}