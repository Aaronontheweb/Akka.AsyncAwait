using Akka.Actor;
using Akka.Event;
using Petabridge.Cmd;

namespace Akka.AsyncAwait.Actors;

public sealed class BatchingActor : ReceiveActor
{
    public static Props Props(int batchSize, IActorRef processor, IActorRef commandEndpoint)
    {
        return Actor.Props.Create(() => new BatchingActor(batchSize, processor, commandEndpoint));
    }
    
    private readonly int _expectedBatchSize;
    private readonly IActorRef _processor;
    private readonly IActorRef _commandEndpoint;
    private readonly HashSet<Messages.Ack> _acks = new HashSet<Messages.Ack>();
    private readonly ILoggingAdapter _log = Context.GetLogger();
    public BatchingActor(int expectedBatchSize, IActorRef processor, IActorRef commandEndpoint)
    {
        _expectedBatchSize = expectedBatchSize;
        _processor = processor;
        _commandEndpoint = commandEndpoint;

        Receive<Messages.Ack>(a =>
        {
            _acks.Add(a);
            _commandEndpoint.Tell(new CommandResponse($"Received {a}", final:false));
            if (_acks.Count >= _expectedBatchSize)
            {
                _commandEndpoint.Tell(new CommandResponse("batch complete"));
                // received all of the messages we expect
                Context.Stop(Self);
            }
        });
    }
}