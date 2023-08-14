public interface IUserRepository
{
    public void Save(UserData obj);
    public UserData Load();
}