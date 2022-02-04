using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using NetCoreServer;
using OpenTap.Cli;


namespace OpenTap.HomeAutomation.Service
{
    [Display("service", Group:"service")]
    public class ServiceCliAction : ICliAction
    {
        private TraceSource log = Log.CreateSource("service");
        
        [CommandLineArgument("client")]
        public bool Client { get; set; }

        public int Execute(CancellationToken cancellationToken)
        {
            if (Client)
            {
                var client = new HttpClient("127.0.0.1", 8080);
                client.Connect();
                var req = new HttpRequest("POST", "http://127.0.0.1/asd");
                req.SetBody("run auto.TapPlan");
                client.SendRequest(req);
                client.Disconnect();
                return 0;
            }
            var server = new ServiceHttpServer("127.0.0.1", 8080);
            server.Start();
            log.Debug("Service started..");
            while (true)
                TapThread.Sleep(100);
            return 0;
        }
    }


    class ServiceHttpServer : HttpServer
    {
        public ServiceHttpServer(IPAddress address, int port) : base(address, port)
        {
        }

        public ServiceHttpServer(string address, int port) : base(address, port)
        {
        }

        public ServiceHttpServer(DnsEndPoint endpoint) : base(endpoint)
        {
        }

        public ServiceHttpServer(IPEndPoint endpoint) : base(endpoint)
        {
        }

        protected override TcpSession CreateSession() => new ServiceHttpSession(this);
    }

    class ServiceHttpSession : HttpSession
    {
        public ServiceHttpSession(HttpServer server) : base(server)
        {
        }

        private static CliActionTree actionParser = new CliActionTree();

        protected override void OnReceivedRequest(HttpRequest request)
        {
            base.OnReceivedRequest(request);
            //this.Response.SetBody("<h1>Hello world!</h1>");
            var body = request.Body;
            var parser = new CommandParser();
            parser.Tokenize($"({body})", out var token);
            // body contains a cli action etc.
            //actionParser.SendResponseAsync(Response.MakeGetResponse("Hello world"));
        }
    }

    internal class CliActionTree
    {
        public string Name { get; }
        public bool IsGroup => (SubCommands?.Count ?? 0) > 0;

        public bool IsBrowsable => (Type?.GetAttribute<BrowsableAttribute>()?.Browsable ?? true) ||
                                   SubCommands.Any(x => x.IsBrowsable);

        public ITypeData Type { get; private set; }
        public List<CliActionTree> SubCommands { get; private set; }

        public CliActionTree Root { get; }

        public CliActionTree()
        {
            var commands = TypeData.GetDerivedTypes(TypeData.FromType(typeof(ICliAction)))
                .Where(t => t.CanCreateInstance && t.GetDisplayAttribute() != null).ToList();
            Name = "tap";
            Root = this;
            foreach (var item in commands)
                ParseCommand(item, item.GetDisplayAttribute().Group, Root);
        }

        CliActionTree(CliActionTree parent, string name)
        {
            Name = name;
        }

        private static void ParseCommand(ITypeData type, string[] group, CliActionTree command)
        {
            if (command.SubCommands == null)
                command.SubCommands = new List<CliActionTree>();

            // If group is not empty. Find command with first group name
            if (group.Length > 0)
            {
                var existingCommand = command.SubCommands.FirstOrDefault(c => c.Name == group[0]);

                if (existingCommand == null)
                {
                    existingCommand = new CliActionTree(command, group[0]);
                    command.SubCommands.Add(existingCommand);
                }

                ParseCommand(type, group.Skip(1).ToArray(), existingCommand);
            }
            else
            {
                command.SubCommands.Add(new CliActionTree(command, type.GetDisplayAttribute().Name)
                    {Type = type, SubCommands = new List<CliActionTree>()});
                command.SubCommands.Sort((x, y) => string.Compare(x.Name, y.Name));
            }
        }

        public CliActionTree GetSubCommand(string[] args)
        {
            if (args.Length == 0)
                return null;

            foreach (var item in SubCommands)
            {
                if (item.Name == args[0])
                {
                    if (args.Length == 1 || item.SubCommands.Any() == false)
                        return item;
                    var subCmd = item.GetSubCommand(args.Skip(1).ToArray());
                    return subCmd ?? item;
                }
            }

            return null;
        }

        /// <summary>
        /// This method calculates the max length in a command tree. Consider the tree outputted by tap help:
        /// run
        /// package
        ///    create
        ///    install  = Longest command (10 characters), this method would return the integer 10.
        /// </summary>
        /// <param name="levelPadding">How much is each level indenting? In the example above, the subcommands to 'package' is indented with 3 characters</param>
        /// <returns>Max character length of commands outputted</returns>
        internal int GetMaxCommandTreeLength(int levelPadding)
        {
            var initial = this == Root ? 0 : levelPadding;
            var x = 0;

            if (SubCommands.Count == 0)
                return x + Name.Length;

            foreach (var cmd in SubCommands)
            {
                int length = cmd.GetMaxCommandTreeLength(levelPadding);
                if (length > x)
                    x = length;
            }

            return x + initial;
        }
    }
}