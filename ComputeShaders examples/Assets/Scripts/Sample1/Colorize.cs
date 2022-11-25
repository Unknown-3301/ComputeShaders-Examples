using System.Collections;
using System.Collections.Generic;
using ComputeShaders;

public class Colorize
{
    public ComputeShader Shader { get; private set; }
    public CSTexture2D Texture { get; private set; }
    public CSStructuredBuffer<Point> Points { get; private set; }
    public CSCBuffer<Sample1Info> Info { get; private set; }
    private Sample1Info info;

    public Colorize(int width, int height, CSStructuredBuffer<Point> pointerSBuffer)
    {
        Shader = new ComputeShader(@"Assets\Scripts\Sample1\Colorizer.compute", "Colorize");
        Texture = Shader.CreateTexture2D(width, height, TextureFormat.R8G8B8A8_UNorm);

        Points = pointerSBuffer.Share(Shader);
        info = new Sample1Info() { PointsNum = pointerSBuffer.Length, Width = width, Height = height };
        Info = Shader.CreateBuffer(info, Sample1Info.Size);

        Shader.SetRWTexture2D(Texture, 0);
        Shader.SetRWStructuredBuffer(Points, 1);
        Shader.SetBuffer(Info, 0);
    }

    public void Run()
    {
        
        Shader.Dispatch(UnityEngine.Mathf.CeilToInt(Texture.Width / 8f), UnityEngine.Mathf.CeilToInt(Texture.Height / 8f), 1);
    }
}
