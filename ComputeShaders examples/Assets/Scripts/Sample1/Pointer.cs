using System.Collections;
using System.Collections.Generic;
using ComputeShaders;

public class Pointer : System.IDisposable
{
    public ComputeShader Shader { get; private set; }
    public CSStructuredBuffer<Point> Points { get; private set; }
    public CSCBuffer<Sample1Info> Info { get; private set; }

    private Sample1Info info;
    private CSDevice device;

    public Pointer(Point[] points, CSDevice device, int width, int height)
    {
        this.device = device;
        Shader = device.CreateComputeShader(@"Assets\Scripts\Sample1\Pointer.compute", "MovePoints");
        Points = device.CreateStructuredBuffer(points, Point.Size, true);

        info = new Sample1Info() { PointsNum = points.Length, Width = width, Height = height };
        Info = device.CreateBuffer(info, Sample1Info.Size);

        
    }

    public CSStructuredBuffer<Point> Run(int randSeed)
    {
        info.dummyVar1 = randSeed;
        Info.UpdateBuffer(info);

        device.SetBuffer(Info, 0);
        device.SetRWStructuredBuffer(Points, 0);
        device.SetComputeShader(Shader);

        device.Dispatch(UnityEngine.Mathf.CeilToInt(Points.Length / 16f), 1, 1);

        return Points;
    }

    public void Dispose()
    {
        Info.Dispose();
        Points.Dispose();
        Shader.Dispose();
    }
}
