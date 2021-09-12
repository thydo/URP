using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using ThreeDeeBear.Models.Ply;

public class PointCloudRender : MonoBehaviour
{
    Texture2D texColor;
    Texture2D texPosScale;
    uint resolution;
    static PlyResult result;

    public VisualEffect vfx;
    public float particleSize = 0.01f;
    bool toUpdate = false;
    uint particleCount;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
        string path = "Assets/Objects/bun_zipper.ply";
        RenderObj(path, false, 1, 2048);
        //RenderObj(1,1);

    }

    private void Update()
    {
        if (toUpdate)
        {
            toUpdate = false;

            vfx.Reinit();
            vfx.SetUInt(Shader.PropertyToID("ParticleCount"), particleCount);
            vfx.SetTexture(Shader.PropertyToID("TexColor"), texColor);
            vfx.SetTexture(Shader.PropertyToID("TexPosScale"), texPosScale);
            vfx.SetUInt(Shader.PropertyToID("Resolution"), resolution);
        }
    }

    public void SetParticles(Vector3[] positions, Color[] colors)
    {
        texColor = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        texPosScale = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        int texWidth = texColor.width;
        int texHeight = texColor.height;

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                int index = x + y * texWidth;
                texColor.SetPixel(x, y, colors[index]);
                var data = new Color(positions[index].x, positions[index].y, positions[index].z, particleSize);
                texPosScale.SetPixel(x, y, data);
            }
        } 

        texColor.Apply();
        texPosScale.Apply();
        particleCount = (uint)positions.Length;
        toUpdate = true;
    }

    private void RenderObj(string path, bool isColor, int scale, uint resolution)
    {
        result = PlyHandler.GetVerticesAndTriangles(path);
        this.resolution = resolution;
        int xCount = result.Vertices.Count;
        Vector3[] positions = new Vector3[xCount];
        Color[] colors = new Color[xCount];
        if (!isColor)
        {
            RandomizeColor(colors);
        }
        else
        {
            result.Colors.CopyTo(colors);
        }
        if (scale < 1)
        {
            result.Vertices.CopyTo(positions);
        }
        else
        {
            ScaleObj(positions, scale);
        }
        
        SetParticles(positions, colors);
    }

    private void RenderObj(uint resolution, uint particleCount)
    {
        this.resolution = resolution;
        this.particleCount = particleCount;
        Vector3[] positions = new Vector3[(int)resolution * (int)resolution];
        Color[] colors = new Color[(int)resolution * (int)resolution];
        RandomizeColor(colors);
        for (int x = 0; x < (int)resolution; x++)
        {
            for (int y = 0; y < (int)resolution; y++)
            {
                positions[x + y * (int)resolution] = new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);
                colors[x + y * (int)resolution] = new Color(Random.value, Random.value, Random.value, 1);
            }
        }
        SetParticles(positions, colors);
    }

    private void RandomizeColor(Color[] colors)
    {
        float rand;
        for (int i = 0; i < colors.Length; i++)
        {
            rand = Random.Range(0.8f,1f);
            colors[i] = new Color(rand, rand, rand, 1); //new Color(Random.value, Random.value, Random.value, 1);
        }
    }

    private void ScaleObj(Vector3[] positions, int scale)
    {
        int count = 0;
        foreach (var i in result.Vertices)
        {
            positions[count] = i * scale;
            count++;
        }
    }

}
