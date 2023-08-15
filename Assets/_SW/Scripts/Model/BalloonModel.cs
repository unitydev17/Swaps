using UnityEngine;

public class BalloonModel
{
    public Vector2 heightRange;
    public Vector2 widthRange;
    public float speed;
    public float amplitude;
    public float period;
    public float scale;

    public Vector3 pos;
    public float time;

    public void Update()
    {
        time += Time.deltaTime * speed;
        pos.x = Mathf.Lerp(widthRange.x, widthRange.y, time);
        pos.y = Mathf.Lerp(heightRange.x, heightRange.y, time);
        pos.y += amplitude * Mathf.Sin(period * time);
        pos.y = Mathf.Max(pos.y, heightRange.x);
    }
}