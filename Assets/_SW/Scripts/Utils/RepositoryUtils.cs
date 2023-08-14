public static class RepositoryUtils
{
    public static void Save(Level level)
    {
        new LevelStorage().Save(level);
    }

    public static Level LoadLevel(int index)
    {
        return new LevelStorage().Load(index);
    }
}