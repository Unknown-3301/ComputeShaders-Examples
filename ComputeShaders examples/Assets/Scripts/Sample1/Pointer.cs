using System.Collections;
using System.Collections.Generic;
using ComputeShaders;

public class Pointer
{
    public ComputeShader Shader { get; private set; }
    public CSStructuredBuffer<Point> Points { get; private set; }
    public CSCBuffer<Sample1Info> Info { get; private set; }
    private Sample1Info info;

    public Pointer(Point[] points, int width, int height)
    {
        Shader = new ComputeShader(@"Assets\Scripts\Sample1\Pointer.compute", "MovePoints");
        Points = Shader.CreateStructuredBuffer(points, Point.Size, true);

        info = new Sample1Info() { PointsNum = points.Length, Width = width, Height = height };
        Info = Shader.CreateBuffer(info, Sample1Info.Size);

        Shader.SetRWStructuredBuffer(Points, 0);
        Shader.SetBuffer(Info, 0);
    }

    public void Run(int randSeed)
    {
        info.dummyVar1 = randSeed;
        Info.UpdateBuffer(info);

        Shader.Dispatch(UnityEngine.Mathf.CeilToInt(Points.Length / 16f), 1, 1);

        Points.Flush();
    }
}
