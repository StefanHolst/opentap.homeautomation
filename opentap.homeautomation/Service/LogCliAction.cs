using System.Threading;
using OpenTap.Cli;
using OpenTap.Diagnostic;

namespace OpenTap.HomeAutomation.Service;

[Display("log", Group: "service")]
public class LogCliAction : ICliAction
{
    [UnnamedCommandLineArgument("name")]
    public string Name { get; set; }

    public int Execute(CancellationToken cancellationToken)
    {
        var plan = ServiceLoadCliAction.LoadedPlans[Name];

        if (RunCliAction.PlanLogs.TryGetValue(plan, out var logs))
        {
            foreach (var evt in logs)
            {
                (Log.Context as ILogContext2).AddEvent(evt);
            }
        }

        return 0;
    }
}