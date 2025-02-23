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
        LeverInteractable.OnLeverAction += OnLeverAction;
        TVBehavior.gameStateChangedTVBehavior += UpdateGameState;
        CustomCollisionEventHandler.GameStateChangedCostomCollisionEventHandler += UpdateGameState;
        PlayerManager.GameStateChangedPlayer += UpdateGameState;
        WinCollision.GameStateChangedWinCollision += UpdateGameState;
        TriggerGameStateOnCollsion.GameStateChangedTriggerGameStateOnCollsion += UpdateGameState;
        AudioDetection.GameStateChangedMegaPhone += UpdateGameState;
    }

    private void OnDestroy()
    {
        CollisionEventHandler.OnWaterStateChangedCable -= OnWaterStateChangedCable;
        CollisionEventHandler.OnWaterStateChangedPlayer -= OnWaterStateChangedPlayer;
        LeverInteractable.OnLeverAction -= OnLeverAction;
        TVBehavior.gameStateChangedTVBehavior -= UpdateGameState;
        CustomCollisionEventHandler.GameStateChangedCostomCollisionEventHandler -= UpdateGameState;
        PlayerManager.GameStateChangedPlayer -= UpdateGameState;
        WinCollision.GameStateChangedWinCollision -= UpdateGameState;
        TriggerGameStateOnCollsion.GameStateChangedTriggerGameStateOnCollsion -= UpdateGameState;
        AudioDetection.GameStateChangedMegaPhone -= UpdateGameState;
    }

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }


    private void OnWaterStateChangedCable(bool state)
    {
      //  Debug.Log($"[GameManager]: OnWaterStateChangedCable: + {state}");
        cableISinWater = state;
        CheckPlayerAndCableInWhater(playerIsInWhater, cableISinWater, electricityIsActive);
    }


    private void OnWaterStateChangedPlayer(bool state)
    {
       
        playerIsInWhater = state;
        CheckPlayerAndCableInWhater(playerIsInWhater, cableISinWater, electricityIsActive);
    }


    private void OnLeverAction(bool state)
    {
  
        electricityIsActive = state;
        CheckPlayerAndCableInWhater(playerIsInWhater, cableISinWater, electricityIsActive);
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
            case GameState.Game:
                break;
            case GameState.Tutorial:
                break;
            case GameState.Menu:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

      //  Debug.Log($"[Game Manager] updating game state to {newState}");
        OnGameStateChanged?.Invoke(newState);
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

    private void CheckPlayerAndCableInWhater(bool playerIsInWhater, bool cableIsInWhater, bool electricityIsActive)
    {
        if (playerIsInWhater == true && cableIsInWhater == true && electricityIsActive == true)
        {
            //TODO: Water Hight loest nicht Trigger aus in CollisonCOllider Script
          //  Debug.Log("[GameManager]: OnWaterStateChangedPlayerAndCableInWhater: ");
            UpdateGameState(GameState.ElectricShock);
        }
    }


    public void RestartGame()
    {
        //Debug.Log($"GameManager: Restart Game + {GameState.}");
        SceneManager.LoadScene("NewWaterScene");
    }
}