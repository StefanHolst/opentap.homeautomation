using System.Collections.Generic;
using System.Threading;
using OpenTap.Cli;

namespace OpenTap.HomeAutomation.Service;

[Display("load", Group: "service")]
public class ServiceLoadCliAction : ICliAction
{
    [UnnamedCommandLineArgument("plan file")]
    public string PlanFile { get; set; }
        
    [CommandLineArgument("load-as")]
    public string Name { get; set; }

    public static Dictionary<string, TestPlan> LoadedPlans = new Dictionary<string, TestPlan>(); 
        
    public int Execute(CancellationToken cancellationToken)
    {
        var plan = TestPlan.Load(PlanFile);
        LoadedPlans[Name ?? plan.Name] = plan;
        return 0;
    }
}