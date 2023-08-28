public interface IFlushWorker : IWorker<Flushes>
{
    void Setup(Board board);
}