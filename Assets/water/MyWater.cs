using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class MyWater : MonoBehaviour
{
    Mesh myMesh;
    MeshFilter meshFilter;
   
    public Shader myShader;
    public Texture2D noiseTex;
    public float noiseScale = 1;
    public float noiseStrength = 1;
    public float4 scrollSpeed = float4.zero;
    public Color waterColor = Color.blue;
    public Color specularColor = Color.white;

    //plane settings
    [SerializeField] Vector2 planeSize = new Vector2(1, 1);
    [SerializeField] int planeResolution = 1;
    
    [Header("Wave 1")]
    [SerializeField] float wave1Speed = 1;
    [SerializeField] float wave1Amplitude = 0.1f;
    [SerializeField] float wave1Length= 1;
    
    [Header("Wave 2")]
    [SerializeField] float wave2Speed = 1;
    [SerializeField] float wave2Amplitude = 0.1f;
    [SerializeField] float wave2Length = 1;
    
    [Header("Wave 3")]
    [SerializeField] float wave3Speed = 1;
    [SerializeField] float wave3Amplitude = 0.1f;
    [SerializeField] float wave3Length = 1;
    
    //mesh values
    List<Vector3> vertices;
    List<int> triangles;

    void Awake()

    {
        myMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;
    }

    void Start()
    {
        GeneratePlane(planeSize, planeResolution);
        
        
        Material myMaterial = new Material(myShader);
        
        myMaterial.SetFloat("_Wave1Amplitude", wave1Amplitude);
        myMaterial.SetFloat("_Wave1Speed", wave1Speed);
        myMaterial.SetFloat("_Wave1Length", wave1Length);
        myMaterial.SetFloat("_Wave2Speed", wave2Speed);
        myMaterial.SetFloat("_Wave2Amplitude", wave2Amplitude);
        myMaterial.SetFloat("_Wave2Length", wave2Length);
        myMaterial.SetFloat("_Wave3Speed", wave3Speed);
        myMaterial.SetFloat("_Wave3Amplitude", wave3Amplitude);
        myMaterial.SetFloat("_Wave3Length", wave3Length);
        myMaterial.SetTexture("_NoiseTex", noiseTex);
        myMaterial.SetFloat("_NoiseScale", noiseScale);
        myMaterial.SetFloat("_NoiseStrength", noiseStrength);
        myMaterial.SetVector("_ScrollSpeed", scrollSpeed);
        myMaterial.SetVector("_Color", waterColor);
        myMaterial.SetVector("_SpecularColor",specularColor);
        myMaterial.SetFloat("_ReflectionStrength", 1);
        myMaterial.SetFloat("_RefractionStrength", 1);
        myMaterial.SetFloat("_FresnelStrength", 2);
        
        GetComponent<Renderer>().material = myMaterial;
        
       
    }

    void Update()
    {
        
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