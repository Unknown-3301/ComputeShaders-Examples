using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sample1Info
{
    public int PointsNum;
    public int Width;
    public int Height;

    public int dummyVar1;

    public static int Size { get => sizeof(int) * 4; }
}
