
using System.Threading.Tasks;

public interface IWorker<T>
{
    public Task<T> Work();
}