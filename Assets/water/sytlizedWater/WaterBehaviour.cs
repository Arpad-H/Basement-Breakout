using System.Collections;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [SerializeField] private float floodingSpeed = 0.5f;
    [SerializeField] private GameObject heightPlane;
    [SerializeField] private GameObject waterSim;
    private bool isFlooding = false; // Steuert, ob das Wasser steigt
    private bool lowerSim = false; // Steuert, ob das Wasser steigt

    private void Awake()
    {
        // Abonniere das GameState-Änderungs-Event
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (isFlooding)
        {
            heightPlane.transform.Translate(Vector3.up * Time.deltaTime * floodingSpeed);
        }
        if (lowerSim)
        {
            waterSim.transform.Translate(Vector3.down * Time.deltaTime * floodingSpeed);
        }
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Game)
        {
            Debug.Log("WaterBehaviour: GameState is now 'Game'. Starting flooding.");
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
        isFlooding = true;
        floodingSpeed = 0.5f *floodingSpeed;
    }
    public void RampUpSpeed()
    {
        lowerSim = true;
        floodingSpeed = floodingSpeed*2;
    }
}