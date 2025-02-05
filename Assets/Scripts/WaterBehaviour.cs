using System.Collections;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [SerializeField] private float floodingSpeed = 0.5f;
    [SerializeField] private GameObject heightPlane;
    [SerializeField] private GameObject waterSim;
    [SerializeField] private GameObject WaveGen;
    [SerializeField] private GameObject PlayerHead;
    [SerializeField] private GameObject Player;
    float drowningtime = 10f;
    float timeUnderWater = 0f;
    
    private bool isFlooding = false; // Steuert, ob das Wasser steigt
    private bool lowerSim = false; // Steuert, ob das Wasser steigt

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
            waterSim.transform.Translate(Vector3.down * (Time.deltaTime * floodingSpeed*2));
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
     
        floodingSpeed = 0.04f;
    }
    public void StartWaveGen()
    {
        Debug.Log("StartWaveGen");
        WaveGen.SetActive(true);
    }
}