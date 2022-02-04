using System.Net;
using System.Reflection;
using Akka.AsyncAwait;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Phobos.Actor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryTracing(b =>
{
    var resource = ResourceBuilder.CreateDefault()
        .AddService(Assembly.GetEntryAssembly().GetName().Name, serviceInstanceId: $"{Dns.GetHostName()}");
    
    // uses the default Jaeger settings
    b.AddJaegerExporter();
    b.SetResourceBuilder(resource);

    // decorate our service name so we can find it when we look inside Jaeger
    b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AspNet", "Demo", serviceInstanceId:Dns.GetHostName()));
    
    // receive traces from built-in sources
    b.AddPhobosInstrumentation();
});

builder.Services.AddOpenTelemetryMetrics(b =>
{
    var resource = ResourceBuilder.CreateDefault()
        .AddService(Assembly.GetEntryAssembly().GetName().Name, serviceInstanceId: $"{Dns.GetHostName()}");
    
    b.SetResourceBuilder(resource)
        .AddPhobosInstrumentation()
        .AddPrometheusExporter(opt =>
        {
        });
});

// run Akka.NET
builder.Services.AddHostedService<AkkaService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.RunAsync();