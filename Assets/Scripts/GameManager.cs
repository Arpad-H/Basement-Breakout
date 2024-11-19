using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour


{
    public static GameManager Instance;
    public static event Action<GameState> OnGameStateChanged;

    void Awake()
    {
        Instance = this;
        CableBehavior.OnWaterStateChanged += OnWaterStateChanged;
    }

    private void OnDestroy()
    {
        CableBehavior.OnWaterStateChanged -= OnWaterStateChanged;
    }


    private void OnWaterStateChanged(bool state)
    {
        Debug.Log("GameManager: OnWaterStateChanged: " + state);
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