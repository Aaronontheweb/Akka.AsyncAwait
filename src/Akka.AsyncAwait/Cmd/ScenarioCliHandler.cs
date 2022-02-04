using Akka.Actor;
using Petabridge.Cmd.Host;

namespace Akka.AsyncAwait.Cmd;

/// <summary>
///     Used to run the <see cref="ScenarioCli.ScenarioCommandPalette" />
/// </summary>
public sealed class ScenarioCliHandler : CommandPaletteHandler
{
    public static readonly ScenarioCliHandler Instance = new ScenarioCliHandler();

    private ScenarioCliHandler() : base(ScenarioCli.ScenarioCommandPalette)
    {
        HandlerProps = Props.Create(() => new ScenarioCliRouter());
    }

    public override Props HandlerProps { get; }
}