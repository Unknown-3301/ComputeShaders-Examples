using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sample2Info
{
    public int Particles;
    public float Width;
    public float Height;

    public int dummyVar1;

    public static int Size { get => sizeof(int) * 2 + sizeof(float) * 2; }
}
