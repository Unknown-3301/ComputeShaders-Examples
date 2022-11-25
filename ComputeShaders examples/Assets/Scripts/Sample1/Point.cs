using UnityEngine;

public struct Point
{
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed;
    public Color Color;

    public static int Size { get => sizeof(float) * 9; }
}
