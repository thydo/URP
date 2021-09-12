using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class VFXGraphRenderer : MonoBehaviour
{
    [SerializeField]
    public VisualEffect vfx;

    [SerializeField]
    int vertexCount;

    [SerializeField]
    public float particleSize = 5;

    [SerializeField]
    int scale;

    uint particleCount;
    List<Vector3> vertices;
    bool toUpdate = false;
    uint resolution = 2048;
    Texture2D texColor;
    Texture2D texPosScale;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] positions = new Vector3[vertexCount];
        Color[] colors = new Color[vertexCount];
        ParseFile();
        RandomizeColor(colors);
        vertices.CopyTo(positions);
        SetParticles(positions, colors);

    }

    void ParseFile()
    {
        TextAsset bunnAsset = Resources.Load<TextAsset>("bun_zipper");
        //Debug.Log(bunnAsset.ToString());
        List<string> plyFile = bunnAsset.text.Split('\n').ToList<string>();//File.ReadLines(objectPath).ToList<string>();
        vertices = new List<Vector3>();
        plyFile.GetRange(0, vertexCount).ForEach(vertex =>
        {
            string[] xyzci = vertex.Split(' ');
            vertices.Add(new Vector3(float.Parse(xyzci[0]), float.Parse(xyzci[1]), float.Parse(xyzci[2])) * scale);
        });
    }
        // Update is called once per frame
    void Update()
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
    private void RandomizeColor(Color[] colors)
    {
        float rand;
        for (int i = 0; i < colors.Length; i++)
        {
            rand = Random.Range(0.8f, 1f);
            colors[i] = new Color(rand, rand, rand, 1); //new Color(Random.value, Random.value, Random.value, 1);
        }
    }
}
