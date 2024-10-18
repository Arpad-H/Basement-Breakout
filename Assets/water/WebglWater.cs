using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class WebglWater : MonoBehaviour
{
    private float deltaTime = 0.0f;
    Mesh myMesh;
    MeshFilter meshFilter;
    Material myMaterial;
    private RenderTexture textureA;
    private RenderTexture textureB;

    public Shader myShader;

    //plane settings
    [SerializeField] Vector2 planeSize = new Vector2(1, 1);
    [SerializeField] int planeResolution = 1;


    //mesh values
    List<Vector3> vertices;
    List<int> triangles;

    void Awake()

    {
        myMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;
    }

    void ClearRenderTexture(RenderTexture rt, Color color)
    {
        RenderTexture.active = rt;
        GL.Clear(false, true, color);
        RenderTexture.active = null;
    }

    void Start()
    {
        GeneratePlane(planeSize, planeResolution);
        textureA = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBFloat);
        textureB = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBFloat);
        ClearRenderTexture(textureA, Color.black);
        ClearRenderTexture(textureB, Color.black);


        myMaterial = new Material(myShader);

        myMaterial.SetFloat("_Strength", 1);
        myMaterial.SetTexture("_MainTex", textureA);
        myMaterial.SetVector("_Delta", new Vector2(1.0f / textureA.width, 1.0f / textureA.height));


        GetComponent<Renderer>().material = myMaterial;

       // AddDrop(new Vector2(5.0f, 5.0f), 4.0f, 1);
    }

    void Update()
    {
        deltaTime += Time.deltaTime;
        // Step the water simulation every frame
        StepSimulation();

      


        if (deltaTime >= 2.0f)
        {
            deltaTime = 0.0f;
            Debug.Log("Adding drop");
            AddDrop(new Vector2(5.0f, 5.0f), 4.0f, 1);
        }
    }

    public void AddDrop(Vector2 center, float radius, float strength)
    {
        center = new Vector2(Random.Range(0, 11), Random.Range(0, 11));
        myMaterial.SetInt("_OperationMode", 0); // Set to Drop Mode
        myMaterial.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));
        myMaterial.SetFloat("_Radius", 0.01f);
        myMaterial.SetFloat("_Strength", 10f);

        
        // Render simulation into textureB
        Graphics.Blit(textureA, textureB, myMaterial);
        SwapTextures();
    }

    public void StepSimulation()
    {
        myMaterial.SetInt("_OperationMode", 1); // Set to Update Mode
        // Render update into textureB
        Graphics.Blit(textureA, textureB, myMaterial);
        SwapTextures();
    }

   

    void SwapTextures()
    {
        RenderTexture temp = textureA;
        textureA = textureB;
        textureB = temp;

        // Update shader to use the new active texture
        myMaterial.SetTexture("_MainTex", textureA);
    }

    void GeneratePlane(Vector2 size, int resolution)
    {
        vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        float xPerStep = size.x / resolution;
        float yPerStep = size.y / resolution;

        for (int y = 0; y < resolution + 1; y++)
        {
            for (int x = 0; x < resolution + 1; x++)
            {
                vertices.Add(new Vector3(x * xPerStep, 0, y * yPerStep));

                float u = (float)x / resolution;
                float v = (float)y / resolution;
                uvs.Add(new Vector2(u, v));
            }
        }


        //Create triangles
        triangles = new List<int>();
        for (int row = 0; row < resolution; row++)
        {
            for (int column = 0; column < resolution; column++)
            {
                int i = (row * resolution) + row + column;

                //first triangle 
                triangles.Add(i);
                triangles.Add(i + (resolution) + 1);
                triangles.Add(i + (resolution) + 2);

                //second triangle 
                triangles.Add(i);
                triangles.Add(i + resolution + 2);
                triangles.Add(i + 1);
            }
        }

        myMesh.vertices = vertices.ToArray();
        myMesh.triangles = triangles.ToArray();
        myMesh.uv = uvs.ToArray();
        myMesh.RecalculateNormals();
    }
}