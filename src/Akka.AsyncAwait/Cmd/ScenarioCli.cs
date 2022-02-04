using Petabridge.Cmd;

namespace Akka.AsyncAwait.Cmd;

/// <summary>
/// The <see cref="CommandPalette"/> used to run await or PipeTo scenarios.
/// </summary>
public static class ScenarioCli
{
    public static readonly CommandDefinition Await = new CommandDefinitionBuilder()
        .WithName("await")
        .WithDescription("Run an operation with an actor that uses await.")
        .WithArgument(b => b.WithName("batchsize").WithDescription("The number of messages to process")
            .WithSwitch("-b").WithSwitch("-B").WithDefaultValues("5", "10", "25", "50").WithDefaultValue("25"))
        .Build();
    
    public static readonly CommandDefinition PipeTo = new CommandDefinitionBuilder()
        .WithName("pipeto")
        .WithDescription("Run an operation with an actor that uses pipto.")
        .WithArgument(b => b.WithName("batchsize").WithDescription("The number of messages to process")
            .WithSwitch("-b").WithSwitch("-B").WithDefaultValues("5", "10", "25", "50").WithDefaultValue("25"))
        .Build();

    public static readonly CommandPalette ScenarioCommandPalette =
        new CommandPalette("scenario", new[] { Await, PipeTo });
}