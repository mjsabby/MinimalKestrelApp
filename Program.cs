namespace MinimalKestrelApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;

    public static class Program
    {
        // Add support for reading raw http bytes
        // Pass Ctrl+C Cancellation Tokens and Server Shutdown stuff
        // Setup Kestrel server limits
        // Add SSL support with certificate file path
        public static async Task Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: MinimalKestrelApp PortNumber");
                return;
            }

            int portNumber = int.Parse(args[0]);

            var defaultEventSourceLoggerFactory = new DefaultEventSourceLoggerFactory();
            var requestHandler = new RequestHandler();
            var requestDelegate = new RequestDelegate(requestHandler.HandleRequest);

            var server = new KestrelServer(new KestrelServerOptionsConfig(new NotImplementedServiceProvider(), portNumber), new SocketTransportFactory(new SocketTransportOptionsConfig(), new ApplicationLifetime(), defaultEventSourceLoggerFactory), defaultEventSourceLoggerFactory);

            await server.StartAsync(new HttpApplication(requestDelegate), CancellationToken.None);

            Console.WriteLine($"Running on port {portNumber} ...");

            Thread.Sleep(Timeout.Infinite);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("Unobserved Exception");
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Ctrl+C");
        }

        private sealed class NotImplementedServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class RequestHandler
        {
            public async Task HandleRequest(HttpContext context)
            {
                await context.Response.WriteAsync("Hello World");
            }
        }
    }
}