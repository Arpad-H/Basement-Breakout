using System.Collections;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [SerializeField] private float floodingSpeed = 0.5f;
    [SerializeField] private GameObject heightPlane;

    [SerializeField] private GameObject waterSim;

    // [SerializeField] private GameObject WaveGen;
    [SerializeField] private GameObject PlayerHead;
    [SerializeField] private GameObject Player;
    float drowningtime = 10f;
    float timeUnderWater = 0f;

    private bool isFlooding = false; // Steuert, ob das Wasser steigt
    private bool lowerSim = false; // Steuert, ob das Wasser steigt

    [Header("Waves")] [SerializeField] private int waveCount = 4;
    [SerializeField] private float waveLength = 1f;
    [SerializeField] private float waveSteepness = 0.094f;
    [SerializeField] private float waveSpeed = 20f;
    [SerializeField] private float Amplitude = 1;

    static uint DCount = 7;

    [SerializeField] private Vector2[] waveDirections =
    {
        new Vector2(0.53f, 0.45f),
        new Vector2(-0.209f, 0.4f),
        new Vector2(-0.125f, 0.592f),
        new Vector2(0.482f, -0.876f),
        new Vector2(-0.729f, -0.694f),
        new Vector2(-0.248f, 0.968f),
        new Vector2(0.844f, -0.538f)
    };

    private static uint LCount = 6;

    private static float[] Lengths =
    {
        3.56f,
        2.85f,
        2.10f,
        1.30f,
        1.10f,
        1.2f
    };

    static uint SCount = 5;

    static float[] SteepnessRange =
    {
        1.0f,
        1.8f,
        1.6f,
        1.25f,
        0.5f
    };

    static uint SpCount = 9;

    static float[] Speeds =
    {
        0.62f,
        -0.8f,
        0.45f,
        -0.75f,
        0.88f,
        0.70f,
        -0.56f,
        0.35f,
        -0.71f
    };

    struct WaveData
    {
        Vector4 wave;
        float speed;
    };

    struct Wave
    {
        float Length;
        float Steepness;
        float Speed;
        float Amplitude;
        Vector2 Direction;
    };

    private const float GRAVITY = 9.81f;

    private void Awake()
    {
        // Abonniere das GameState-Änderungs-Event
        //   GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        // GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (isFlooding)
        {
            heightPlane.transform.Translate(Vector3.up * (Time.deltaTime * floodingSpeed));
        }

        if (lowerSim)
        {
            waterSim.transform.Translate(Vector3.down * (Time.deltaTime * floodingSpeed));
        }
    }

    public void HandleGameStateChanged(GameManager.GameState newState)
    {
        // Debug.Log("WaterBehaviour: GameState changed to " + newState);
        if (newState == GameManager.GameState.Game)
        {
            // Debug.Log("WaterBehaviour: GameState is now 'Game'. Starting flooding.");
            waterSim.SetActive(true);
            isFlooding = true;
        }
        else
        {
            // Debug.Log($"WaterBehaviour: GameState changed to {newState}. Flooding stopped.");
            isFlooding = false;
        }
    }

    public void SwapSimWithCrest()
    {
        waterSim.SetActive(false);

        floodingSpeed = 0.01f;
    }

    public void RampUpSpeed()
    {
        Debug.Log("RampUpSpeed");
        lowerSim = true;
        isFlooding = true;

        floodingSpeed = 0.08f;
    }

    // public void StartWaveGen()
    // {
    //     Debug.Log("StartWaveGen");
    //     WaveGen.SetActive(true);
    // }
    public Vector3 GetLocalVertexPosition(Vector3 myPos, bool applyRipple)
    {
        return transform.position;
    }

    public Vector3 GetWaveDisplacement(Vector3 worldPos, float time)
    {
        Vector3 displacement = Vector3.zero;

        for (int i = 0; i < waveCount; i++)
        {
            float steepnessMul = Mathf.Lerp(1.0f, 0.1f, (1.0f / 32.0f) * i);
            float length = Lengths[i % LCount] * waveLength;
            float steepness = SteepnessRange[i % SCount] * waveSteepness * steepnessMul;
            float speed = Speeds[i % SpCount] * waveSpeed;
            Vector2 direction = waveDirections[i % DCount].normalized;

            float dispersion = 6.28318f / length;
            float c = Mathf.Sqrt(GRAVITY / dispersion) * speed;
            float f = dispersion * (Vector2.Dot(direction, new Vector2(worldPos.x, worldPos.z)) - c * time);

            float sinF = Mathf.Sin(f);
            float cosF = Mathf.Cos(f);

            float a = steepness / (dispersion * 1.5f);
            float wKA = a * dispersion;

            displacement.x += direction.x * (a * cosF);
            displacement.z += direction.y * (a * cosF);
            displacement.y += a * sinF;
        }

      
        return Amplitude * displacement + transform.position;
    }
}