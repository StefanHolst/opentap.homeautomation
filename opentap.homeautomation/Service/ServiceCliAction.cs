using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using NetCoreServer;
using OpenTap.Cli;
using OpenTap.Diagnostic;

namespace OpenTap.HomeAutomation.Service
{
    [Display("service", Group: "service")]
    public class ServiceCliAction : ICliAction
    {
        private TraceSource log = Log.CreateSource("service");

        [CommandLineArgument("client")] public bool Client { get; set; }
        
        [UnnamedCommandLineArgument("client-command")]
        public string ClientCommand { get; set; }

        public int Execute(CancellationToken cancellationToken)
        {
            if (Client)
            {
                if (string.IsNullOrWhiteSpace(ClientCommand))
                {
                    throw new InvalidOperationException("Client command must be set");
                }
                var client = new HttpClientEx("127.0.0.1", 8080);
                client.Connect();
                var req = new HttpRequest("POST", "http://127.0.0.1/asd");
                req.SetBody(ClientCommand);
                var response = client.SendPostRequest("http://127.0.0.1/asd", ClientCommand).Result.Body;
                foreach (var line in response.Split("\n"))
                {
                    log.Info(line);    
                }
                
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
            List<string> args = new List<string>();
            var car = token.Car;
            while (car != null)
            {
                args.Add(car.Data);
                car = car.Cdr;
            }

            var listener = new HttpTraceListener();
            using (Session.Create(SessionOptions.RedirectLogging))
            {
                Log.AddListener(listener);
                CliActionExecutor.Execute(args.ToArray());
                Log.Flush();
            }

            var logLines = listener.LogEvents.Where(x => x.EventType != (int)LogEventType.Debug).Select(x => x.Message).ToArray();
            SendResponseAsync(Response.MakeGetResponse(System.Text.Json.JsonSerializer.Serialize(logLines)));
        }

        class HttpTraceListener : ILogListener
        {
            public List<Event> LogEvents = new List<Event>();
            public void EventsLogged(IEnumerable<Event> Events)
            {
                LogEvents.AddRange(Events);
            }

            public void Flush()
            {
                
            }
        }

    }
}