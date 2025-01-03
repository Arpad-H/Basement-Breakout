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
    private bool electricityIsActive = true;
   

    void Awake()
    {
        //Instance = this;
        CollisionEventHandler.OnWaterStateChangedCable += OnWaterStateChangedCable;
        CollisionEventHandler.OnWaterStateChangedPlayer += OnWaterStateChangedPlayer;
        LeverInteractable.onLeverAction += OnLeverAction;
        TVBehavior.gameStateChangedTVBehavior += UpdateGameState;
        CustomCollisionEventHandler.GameStateChangedCostomCollisionEventHandler += UpdateGameState;
        PlayerManager.GameStateChangedPlayer += UpdateGameState;
    }

    private void OnDestroy()
    {
        CollisionEventHandler.OnWaterStateChangedCable -= OnWaterStateChangedCable;
        CollisionEventHandler.OnWaterStateChangedPlayer -= OnWaterStateChangedPlayer;
        LeverInteractable.onLeverAction -= OnLeverAction;
        TVBehavior.gameStateChangedTVBehavior -= UpdateGameState;
        CustomCollisionEventHandler.GameStateChangedCostomCollisionEventHandler -= UpdateGameState;
        PlayerManager.GameStateChangedPlayer -= UpdateGameState;
    }

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }


    private void OnWaterStateChangedCable(bool state)
    {
        Debug.LogError($"[GameManager]: OnWaterStateChangedCable: + {state}");
        cableISinWater = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableISinWater, electricityIsActive);
    }
    

    private void OnWaterStateChangedPlayer(bool state)
    {
        Debug.LogError("[GameManager]: OnWaterStateChangedPlayer: " + state);
        playerIsInWhater = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableISinWater, electricityIsActive);
    }


    private void OnLeverAction(bool state)
    {
        Debug.LogError("[GameManager]: electricityIsActive: " + state);
        electricityIsActive = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableISinWater, electricityIsActive);
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
        Debug.Log($"[Game Manager] updating game state to {newState}");
    }


    public enum GameState
    {
        Drowned,
        ElectricShock,
        Win,
        Beam,
        Tutorial,
        Game,
        Menu
    }

    private void checkPlayerAndCableInWhater(bool playerIsInWhater, bool cableIsInWhater, bool electricityIsActive)
    {
        if (playerIsInWhater == true && cableIsInWhater == true && electricityIsActive == true)
        {
            //TODO: Water Hight loest nicht Trigger aus in CollisonCOllider Script
            Debug.Log("[GameManager]: OnWaterStateChangedPlayerAndCableInWhater: " );
           UpdateGameState(GameState.ElectricShock);
           
        }
    }
    
    
    public void RestartGame()
    {
        SceneManager.LoadScene("MAIN");
    }
}

