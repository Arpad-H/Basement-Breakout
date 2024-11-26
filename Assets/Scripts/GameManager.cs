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

    private LeverInteractable leverInteractable;

    private bool playerIsInWhater = false;
    private bool cableIsinWater = false;
    private bool electricityIsActiv = true;


    void Awake()
    {
        //Instance = this;
        CollisionEventHandler.OnWaterStateChangedCable += OnWaterStateChangedCable;
        CollisionEventHandler.OnWaterStateChangedPlayer += OnWaterStateChangedPlayer;
    }

    void Start()
    {
        if (leverInteractable != null)
        {
            leverInteractable.onLeverActivated.AddListener(OnLeverActivated);
            leverInteractable.onLeverDeactivated.AddListener(OnLeverDeactivated);
        }
    }


    private void OnDestroy()
    {
        CollisionEventHandler.OnWaterStateChangedCable -= OnWaterStateChangedCable;
        CollisionEventHandler.OnWaterStateChangedPlayer -= OnWaterStateChangedPlayer;


        if (leverInteractable != null)
        {
            leverInteractable.onLeverActivated.RemoveListener(OnLeverActivated);
            leverInteractable.onLeverDeactivated.RemoveListener(OnLeverDeactivated);
        }
    }


    private void OnWaterStateChangedCable(bool state)
    {
        Debug.Log($"[GameManager]: OnWaterStateChangedCable: + {state}");
        cableIsinWater = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableIsinWater);
    }

    private void OnWaterStateChangedPlayer(bool state)
    {
        Debug.Log("[GameManager]: OnWaterStateChangedPlayer: " + state);
        playerIsInWhater = state;
        checkPlayerAndCableInWhater(playerIsInWhater, cableIsinWater);
    }

    private void OnLeverActivated()
    {
        Debug.Log("[GameManager]: OnLeverActivated");
        electricityIsActiv = true;
    }

    private void OnLeverDeactivated()
    {
        Debug.Log("[GameManager]: OnLeverDeactivated");
        electricityIsActiv = false;
    }


    public void UpdateGameState(GameState newState)
    {
        //State = newState;
        switch (newState)
        {
            case GameState.ElectricShock:
                break;
            case GameState.Drowned:
                SceneManager.LoadScene("startMenuScene");
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

    private void checkPlayerAndCableInWhater(bool playerIsInWhater, bool cableIsInWhater)
    {
        if (playerIsInWhater == true && cableIsInWhater == true && electricityIsActiv == true)
        {
            Debug.Log("[GameManager]: GameState: Downed ");
            UpdateGameState(GameState.Drowned);

            //TODO: GameOverScene
        }
    }
}