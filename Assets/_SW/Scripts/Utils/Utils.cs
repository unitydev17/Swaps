using UnityEngine;

public static class Utils
{
    public static float Random(this Vector2 vec)
    {
        return UnityEngine.Random.Range(vec.x, vec.y);
    }

    public static int SecondsToMilliseconds(this float value)
    {
        return (int) System.TimeSpan.FromSeconds(value).TotalMilliseconds;
    }
}