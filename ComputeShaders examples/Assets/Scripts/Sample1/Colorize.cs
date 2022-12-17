using System.Collections;
using System.Collections.Generic;
using ComputeShaders;

public class Colorize : System.IDisposable
{
    public ComputeShader Shader { get; private set; }
    public CSTexture2D Texture { get; private set; }
    public CSCBuffer<Sample1Info> Info { get; private set; }

    private Sample1Info info;
    private CSDevice device;

    public Colorize(CSDevice device, int width, int height, int pointsNum)
    {
        this.device = device;
        Shader = device.CreateComputeShader(@"Assets\Scripts\Sample1\Colorizer.compute", "Colorize");
        Texture = device.CreateTexture2D(width, height, TextureFormat.R8G8B8A8_UNorm);

        info = new Sample1Info() { PointsNum = pointsNum, Width = width, Height = height };
        Info = device.CreateBuffer(info, Sample1Info.Size);

        
    }

    public void Run(CSStructuredBuffer<Point> points)
    {
        device.SetRWTexture2D(Texture, 0);
        device.SetRWStructuredBuffer(points, 1);
        device.SetBuffer(Info, 0);

        device.SetComputeShader(Shader);

        device.Dispatch(UnityEngine.Mathf.CeilToInt(Texture.Width / 8f), UnityEngine.Mathf.CeilToInt(Texture.Height / 8f), 1);
    }

    public void Dispose()
    {
        Texture.Dispose();
        Info.Dispose();
        Shader.Dispose();
    }
}
