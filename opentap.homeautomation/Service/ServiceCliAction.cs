using System.Net;
using System.Threading;
using NetCoreServer;
using OpenTap.Cli;


namespace OpenTap.HomeAutomation.Service
{
    [Display("service")]
    public class ServiceCliAction : ICliAction
    {
        private TraceSource log = Log.CreateSource("service");
        public int Execute(CancellationToken cancellationToken)
        {
            var server = new ServiceHttpServer("127.0.0.1", 8080);
            server.Start();
            log.Debug("Service started..");
            while(true)
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

        protected override void OnReceivedRequest(HttpRequest request)
        {
            base.OnReceivedRequest(request);
            //this.Response.SetBody("<h1>Hello world!</h1>");
            var body = request.Body;
            // body contains a cli action etc.
            
            
            SendResponseAsync(Response.MakeGetResponse("Hello world"));
        }
    }
}