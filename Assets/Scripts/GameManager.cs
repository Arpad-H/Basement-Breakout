using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour


{
    public static GameManager Instance;
    public static event Action<GameState> OnGameStateChanged;
    
    private bool playerIsInWhater = false;
    private bool cableISinWater = false;
    
    // Aktueller GameState
    private GameState currentState;
   
    

    void Awake()
    {
        //Instance = this;
        CollisionEventHandler.OnWaterStateChangedCable += OnWaterStateChangedCable;
        PlayerCollisionHEventHandler.OnWaterStateChangedPlayer += OnWaterStateChangedCable;
        TVBehavior.gameStateChanged += UpdateGameState;
        
        SetInitialGameState();
    }

    private void OnDestroy()
    {
        CollisionEventHandler.OnWaterStateChangedCable -= OnWaterStateChangedCable;
        PlayerCollisionHEventHandler.OnWaterStateChangedPlayer -= OnWaterStateChangedCable;
        TVBehavior.gameStateChanged -= UpdateGameState;
    }


    private void OnWaterStateChangedCable(bool state)
    {
        Debug.Log("GameManager: OnWaterStateChanged: " + state);
        cableISinWater = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableISinWater);
    }

    private void OnWaterStateChangedPlayer(bool state)
    {
        Debug.Log("GameManager: OnWaterStateChangedPlayer: " + state);
        playerIsInWhater = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableISinWater);
    }
    
    private void SetInitialGameState()
    {
        // Setzt den initialen Zustand des Spiels auf Tutorial
        UpdateGameState(GameState.Tutorial);
    }

    

    public void UpdateGameState(GameState newState)
    {
        if (currentState == newState)
        {
            Debug.Log($"GameManager: GameState already set to {newState}. No changes made.");
            return;
        }

        currentState = newState;

        Debug.Log($"GameManager: GameState updated to {currentState}");

        // Verarbeite Logik basierend auf dem neuen State
        switch (newState)
        {
            case GameState.ElectricShock:
                break;
            case GameState.Drowned:
                break;
            case GameState.Win:
                break;
            case GameState.Beam:
                break;
            case GameState.Game:
                Debug.Log("GameManager: Game state detected. Starting Gameplay.");
                break;
            case GameState.Tutorial:
                Debug.Log("GameManager: Tutorial state detected. Preparing Tutorial.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        // Andere GameObjekte können hierrauf reagieren
        OnGameStateChanged?.Invoke(newState);
    }


    public enum GameState
    {
        Drowned,
        ElectricShock,
        Win,
        Beam,
        Tutorial,
        Game
    }

    private void checkPlayerAndCableInWhater(bool playerIsInWhater, bool cableIsInWhater)
    {
        if (playerIsInWhater == true && cableIsInWhater == true)
        {
            Debug.Log("GameManager: OnWaterStateChangedPlayerAndCableInWhater: " );
            //TODO: GameOverScene
            SceneManager.LoadScene("startMenuScene");
        }
    }
}