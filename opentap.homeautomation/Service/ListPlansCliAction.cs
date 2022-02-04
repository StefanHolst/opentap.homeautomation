using System.IO;
using System.Threading;
using OpenTap.Cli;

namespace OpenTap.HomeAutomation.Service;

[Display("list-plans", "lists the test plans in the folder", "service")]
public class ListPlansCliAction : ICliAction
{
    private TraceSource log = Log.CreateSource("get-plans");
    public int Execute(CancellationToken cancellationToken)
    {
        var dir = Path.GetDirectoryName(typeof(TestPlan).Assembly.Location);
        var plans = Directory.GetFiles(dir, "*.TapPlan");
        foreach (var plan in plans)
        {
            log.Info($"{Path.GetFileNameWithoutExtension(plan)}");
        }

        return 0;
    }
}