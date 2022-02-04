using System;

namespace OpenTap.HomeAutomation.Scheduling
{
    /// <summary>
    /// This kind of triggered event can calculate the time until it should be triggered to start the next time.
    /// </summary>
    public interface ITimeTriggeredStep : ITestStep
    {
        /// <summary> How much time until triggering the event. </summary>
        TimeSpan TimeToTrigger { get; }
    }
}