using Akka.Actor;
using Akka.AsyncAwait.Cmd;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Petabridge.Cmd.Host;
using Phobos.Actor;
using Phobos.Actor.Configuration;

namespace Akka.AsyncAwait;

public class AkkaService : IHostedService
{
    private readonly IServiceProvider _provider;
    private ActorSystem _actorSystem;

    public AkkaService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var metrics = _provider.GetRequiredService<MeterProvider>();
        var tracer = _provider.GetRequiredService<TracerProvider>();

        var phobosSetup = PhobosSetup.Create(new PhobosConfigBuilder()
                .WithMetrics(m =>
                    m.SetMetricsRoot(metrics)) // binds Phobos to same IMetricsRoot as ASP.NET Core
                .WithTracing(t => t.SetTracer(tracer))) // binds Phobos to same tracer as ASP.NET Core
            .WithSetup(BootstrapSetup.Create()
                .WithActorRefProvider(PhobosProviderSelection
                    .Local)); // last line activates Phobos inside Akka.NET

        _actorSystem = ActorSystem.Create("LocalSys", phobosSetup);

        var pbm = PetabridgeCmd.Get(_actorSystem);
        pbm.RegisterCommandPalette(ScenarioCliHandler.Instance);
        pbm.Start();

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _actorSystem.Terminate().ConfigureAwait(false);
    }
}