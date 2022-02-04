using Akka.Actor;
using Akka.AsyncAwait.Actors;
using Petabridge.Cmd;
using Petabridge.Cmd.Host;
using Props = Akka.Actor.Props;

namespace Akka.AsyncAwait.Cmd;

public sealed class ScenarioCliRouter : CommandHandlerActor
{
    public ScenarioCliRouter() : base(ScenarioCli.ScenarioCommandPalette)
    {
        Process(ScenarioCli.Await.Name, (command, arguments) =>
        {
            var batchSize = int.Parse(arguments.ArgumentValues("batchsize").Single());

            var awaitActor = Context.ActorOf(Props.Create(() => new AsyncAwaitActor()));
            var batchingActorProps = BatchingActor.Props(batchSize, awaitActor, Sender);
            var batchActor = Context.ActorOf(batchingActorProps);
            Sender.Tell(new CommandResponse($"Starting async / await batch of [{batchSize}]", false));
            foreach (var a in Enumerable.Range(0, batchSize))
            {
                awaitActor.Tell(new Messages.Req(a), batchActor);
            }
        });
        
        Process(ScenarioCli.PipeTo.Name, (command, arguments) =>
        {
            var batchSize = int.Parse(arguments.ArgumentValues("batchsize").Single());

            var pipeToActor = Context.ActorOf(Props.Create(() => new PipeToActor()));
            var batchingActorProps = BatchingActor.Props(batchSize, pipeToActor, Sender);
            var batchActor = Context.ActorOf(batchingActorProps);
            Sender.Tell(new CommandResponse($"Starting PipeTo batch of [{batchSize}]", false));
            foreach (var a in Enumerable.Range(0, batchSize))
            {
                pipeToActor.Tell(new Messages.Req(a), batchActor);
            }
        });
    }
}