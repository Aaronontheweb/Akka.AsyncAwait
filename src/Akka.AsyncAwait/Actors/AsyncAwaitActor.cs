using Akka.Actor;

namespace Akka.AsyncAwait.Actors;

public sealed class AsyncAwaitActor : ReceiveActor
{
    public AsyncAwaitActor()
    {
        Receive<Messages.Batch>(batch =>
        {
            foreach (var b in Enumerable.Range(0, batch.Size))
            {
                // preserve the ref of the original sender
                Self.Forward(new Messages.Req(b));
            }
        });
        
        ReceiveAsync<Messages.Req>(async r =>
        {
            var ack = r.Ack;
            await Task.Delay(1);
            Sender.Tell(ack);
        });
    }
}