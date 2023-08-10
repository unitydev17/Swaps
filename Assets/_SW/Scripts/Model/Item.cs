using UnityEngine;

public class Item : MonoBehaviour
{
    public int x;
    public int y;

    public void SetPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}