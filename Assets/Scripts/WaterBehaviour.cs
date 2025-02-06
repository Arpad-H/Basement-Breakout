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
        Debug.Log("WaterBehaviour: GameState changed to " + newState);
        if (newState == GameManager.GameState.Game)
        {
            Debug.Log("WaterBehaviour: GameState is now 'Game'. Starting flooding.");
            waterSim.SetActive(true);
            isFlooding = true;
        }
        else
        {
            Debug.Log($"WaterBehaviour: GameState changed to {newState}. Flooding stopped.");
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
            float length = waveLength;
            float steepness = waveSteepness;
            float speed = waveSpeed;
            Vector2 direction = waveDirections[i];

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
        Debug.Log("Displacement: " + displacement);
        return displacement + transform.position;
    }
}