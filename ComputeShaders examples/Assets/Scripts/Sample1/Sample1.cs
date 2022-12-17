using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample1 : MonoBehaviour
{
    [SerializeField] [Min(1)] int pointsNum;

    int width, height;
    Texture2D output;

    ComputeShaders.CSDevice device;
    Colorize colorize;
    Pointer pointer;
    CSToUnityConverter converter;

    private void Start()
    {
        width = Screen.width;
        height = Screen.height;

        Point[] points = new Point[pointsNum];
        for (int i = 0; i < pointsNum; i++)
        {
            float ang = Random.Range(0, 2 * Mathf.PI);

            points[i] = new Point()
            {
                Position = new Vector2(Random.Range(0, width), Random.Range(0, height)),
                Direction = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)),
                Speed = 3,
                Color = Color.HSVToRGB(Random.Range(0, 1f), 0.85f, 0.85f),
            };
        }

        device = new ComputeShaders.CSDevice();
        pointer = new Pointer(points, device, width, height);
        colorize = new Colorize(device, width, height, pointsNum);
        colorize.Texture.EnableCPU_Raw_ReadWrite();

        output = new Texture2D(width, height);
        converter = new CSToUnityConverter(width, height);
    }

    private void Update()
    {
        colorize.Run(pointer.Run(Time.frameCount));

        colorize.Texture.ReadFromRawData(box =>
        {
            converter.Convert(box, output);
        });
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(output, destination);
    }

    private void OnDestroy()
    {
        pointer.Dispose();
        colorize.Dispose();
        device.Dispose();
    }
}
