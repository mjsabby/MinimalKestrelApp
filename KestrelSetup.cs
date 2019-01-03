namespace MinimalKestrelApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
    using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal sealed class HttpApplication : IHttpApplication<HttpContext>
    {
        private readonly RequestDelegate application;

        public HttpApplication(RequestDelegate application) => this.application = application;

        public HttpContext CreateContext(IFeatureCollection contextFeatures) => new DefaultHttpContext(contextFeatures);

        public Task ProcessRequestAsync(HttpContext context) => this.application(context);

        public void DisposeContext(HttpContext context, Exception exception)
        {
        }
    }

    internal sealed class ApplicationLifetime : IApplicationLifetime
    {
        public CancellationToken ApplicationStarted { get; }

        public CancellationToken ApplicationStopping { get; }

        public CancellationToken ApplicationStopped { get; }

        public void StopApplication()
        {
        }
    }

    internal sealed class KestrelServerOptionsConfig : IOptions<KestrelServerOptions>
    {
        public KestrelServerOptionsConfig(IServiceProvider serviceProvider, int port)
        {
            this.Value = new KestrelServerOptions
            {
                AddServerHeader = false,
                AllowSynchronousIO = false,
                ApplicationServices = serviceProvider,
                ApplicationSchedulingMode = SchedulingMode.Default,
                ConfigurationLoader = null
            };

            this.Value.ConfigureEndpointDefaults(options =>
            {
                options.Protocols = HttpProtocols.Http1;
                options.NoDelay = true;
            });

            this.Value.ListenAnyIP(port);
        }

        public KestrelServerOptions Value { get; }
    }

    internal sealed class SocketTransportOptionsConfig : IOptions<SocketTransportOptions>
    {
        public SocketTransportOptionsConfig()
        {
            this.Value = new SocketTransportOptions();
        }

        public SocketTransportOptions Value { get; }
    }
    
    internal sealed class DefaultEventSourceLogger : ILogger
    {
        private readonly FakeDisposable fakeDisposable = new FakeDisposable();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this.fakeDisposable;
        }

        private sealed class FakeDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    internal sealed class DefaultEventSourceLoggerFactory : ILoggerFactory
    {
        private readonly ILogger defaultLogger = new DefaultEventSourceLogger();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return this.defaultLogger;
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class DefaultLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            throw new NotImplementedException();
        }
    }
}