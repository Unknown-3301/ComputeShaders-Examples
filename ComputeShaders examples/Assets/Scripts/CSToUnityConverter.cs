using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComputeShaders;

public class CSToUnityConverter
{
    int originalWidth, editedWidth, Height;

    Texture2D tex_16;

    public CSToUnityConverter(int width, int height)
    {
        originalWidth = width;
        Height = height;
        editedWidth = Mathf.CeilToInt(width / 16f) * 16;

        tex_16 = new Texture2D(editedWidth, Height, UnityEngine.TextureFormat.RGBA32, false);
    }

    public void Convert(TextureDataBox box, Texture2D destination)
    {
        tex_16.LoadRawTextureData(box.DataPointer, editedWidth * Height * 32);
        tex_16.Apply();

        Graphics.CopyTexture(tex_16, 0, 0, 0, 0, originalWidth, Height, destination, 0, 0, 0, 0);
    }
}

