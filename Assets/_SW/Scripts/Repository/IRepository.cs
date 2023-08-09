public interface IRepository
{
    public void Save(Level level);
    public Level Load(int index);
}