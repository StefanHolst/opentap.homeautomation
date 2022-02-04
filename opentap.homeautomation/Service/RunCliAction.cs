using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenTap.Cli;
using OpenTap.Diagnostic;

namespace OpenTap.HomeAutomation.Service;

[Display("run", Group: "service")]
public class RunCliAction : ICliAction
{
    [UnnamedCommandLineArgument("name")]
    public string Name { get; set; }

    public static Dictionary<TestPlan, Task<TestPlanRun>> currentRuns = new Dictionary<TestPlan, Task<TestPlanRun>>();

    public static Dictionary<TestPlan, List<Event>> PlanLogs =
        new Dictionary<TestPlan, List<Event>>();
    public int Execute(CancellationToken cancellationToken)
    {
        var plan = ServiceLoadCliAction.LoadedPlans[Name];
        if (currentRuns.TryGetValue(plan, out var currentRun))
        {
            if (currentRun.IsCompleted == false)
            {
                return 1;
            }
        }

        using (Session.Create(SessionOptions.RedirectLogging))
        {
            var tl = new HttpTraceListener();
            PlanLogs[plan] = tl.LogEvents;
            Log.AddListener(tl);
            currentRuns[plan] = plan.ExecuteAsync();
        }

        return 0;
    }

    [Display("stop", Group: "service")]
    public class StopPlanCliAction : ICliAction
    {
        [UnnamedCommandLineArgument("name")]
        public string Name { get; set; }

        public int Execute(CancellationToken cancellationToken)
        {
            var plan = ServiceLoadCliAction.LoadedPlans[Name];
            RunCliAction.currentRuns[plan].Result.MainThread.Abort();
            return 0;
        }
    }
}