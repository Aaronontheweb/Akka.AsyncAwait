namespace Akka.AsyncAwait.Actors;

public static class Messages
{
    public readonly struct Ack
    {
        public Ack(int seqNo)
        {
            SeqNo = seqNo;
        }

        public int SeqNo { get; }
    }
    
    public record Req(int SeqNo)
    {
        public Req Next()
        {
            return this with { SeqNo = SeqNo + 1 };
        }

        public Ack Ack => new Ack(SeqNo);
    }

    public record Batch
    {
        public int Size { get; init; }
    }
}