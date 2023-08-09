public static class RepositoryUtils
{
    public static void Save(Level level)
    {
        new AssetStorage().Save(level);
    }
    
    public static Level LoadLevel(int index)
    {
        return new AssetStorage().Load(index);
    }
}