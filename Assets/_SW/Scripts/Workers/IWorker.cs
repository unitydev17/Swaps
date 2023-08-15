public interface IWorker<out T>
{
    public T Work();
}