using Akka.Actor;
using Akka.Event;

namespace Akka.AsyncAwait.Actors;

public sealed class BatchingActor : ReceiveActor
{
    public static (TaskCompletionSource tcs, Props) Props(int batchSize, IActorRef processor)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        return (tcs, Akka.Actor.Props.Create(() => new BatchingActor(batchSize, processor, tcs)));
    }
    
    private readonly int _expectedBatchSize;
    private readonly IActorRef _processor;
    private readonly HashSet<Messages.Ack> _acks = new HashSet<Messages.Ack>();
    private readonly ILoggingAdapter _log = Context.GetLogger();
    private readonly TaskCompletionSource _taskCompletionSource;

    public BatchingActor(int expectedBatchSize, IActorRef processor, TaskCompletionSource taskCompletionSource)
    {
        _expectedBatchSize = expectedBatchSize;
        _processor = processor;
        _taskCompletionSource = taskCompletionSource;

        Receive<Messages.Ack>(a =>
        {
            _acks.Add(a);
            if (_acks.Count >= _expectedBatchSize)
            {
                // received all of the messages we expect
                Context.Stop(Self);
            }
        });
    }

    protected override void PostStop()
    {
        _taskCompletionSource.TrySetResult();
    }
}