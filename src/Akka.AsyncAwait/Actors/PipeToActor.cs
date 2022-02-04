using Akka.Actor;

namespace Akka.AsyncAwait.Actors;

public sealed class PipeToActor : ReceiveActor
{
    public PipeToActor()
    {
        Receive<Messages.Batch>(batch =>
        {
            foreach (var b in Enumerable.Range(0, batch.Size))
            {
                // preserve the ref of the original sender
                Self.Forward(new Messages.Req(b));
            }
        });
        
        Receive<Messages.Req>(r =>
        {
            var ack = r.Ack;
            Task.Delay(1).ContinueWith(tr => ack).PipeTo(Sender, Self);
        });
    }
}