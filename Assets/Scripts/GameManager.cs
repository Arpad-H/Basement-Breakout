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
    

    void Awake()
    {
        //Instance = this;
        CollisionEventHandler.OnWaterStateChangedCable += OnWaterStateChangedCable;
        PlayerCollisionHEventHandler.OnWaterStateChangedPlayer += OnWaterStateChangedCable;
    }

    private void OnDestroy()
    {
        CollisionEventHandler.OnWaterStateChangedCable -= OnWaterStateChangedCable;
        PlayerCollisionHEventHandler.OnWaterStateChangedPlayer -= OnWaterStateChangedCable;
    }


    private void OnWaterStateChangedCable(bool state)
    {
        Debug.Log("GameManager: OnWaterStateChanged: " + state);
        cableISinWater = state;
        SceneManager.LoadScene("startMenuScene");
    }

    private void OnWaterStateChangedPlayer(bool state)
    {
        Debug.Log("GameManager: OnWaterStateChangedPlayer: " + state);
        playerIsInWhater = state;
        
    }
    

    public void UpdateGameState(GameState newState)
    {
        //State = newState;
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
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        // Andere GameObjekte können hierrauf reagieren
        OnGameStateChanged?.Invoke(newState);
        {
        }
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
}