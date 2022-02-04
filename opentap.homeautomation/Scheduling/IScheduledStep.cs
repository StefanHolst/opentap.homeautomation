using System;

namespace OpenTap.HomeAutomation
{
    public interface IScheduledStep : ITestStep
    {
        TimeSpan DelayFromNow { get; }
    }
}