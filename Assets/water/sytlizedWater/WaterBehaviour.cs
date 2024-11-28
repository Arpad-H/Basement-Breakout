using System.Collections;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [SerializeField] private float floodingSpeed = 0.5f;
    private bool isFlooding = false; // Steuert, ob das Wasser steigt

    private void Awake()
    {
        // Abonniere das GameState-Änderungs-Event
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        // Entferne das Abonnement, wenn das Objekt zerstört wird
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        // Lasse das Wasser nur steigen, wenn isFlooding true ist
        if (isFlooding)
        {
            transform.Translate(Vector3.up * Time.deltaTime * floodingSpeed);
        }
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        // Überprüfe, ob der GameState auf "Game" geändert wurde
        if (newState == GameManager.GameState.Game)
        {
            Debug.Log("WaterBehaviour: GameState is now 'Game'. Starting flooding.");
            isFlooding = true; // Aktiviert das Steigen des Wassers
        }
        else
        {
            Debug.Log($"WaterBehaviour: GameState changed to {newState}. Flooding stopped.");
            isFlooding = false; // Deaktiviert das Steigen des Wassers
        }
    }
}