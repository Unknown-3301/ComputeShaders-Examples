using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Particle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Acceleration;
    public Color Color;
    public int Type;

    public static int Size { get => sizeof(float) * 10 + sizeof(int); }
}
