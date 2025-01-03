using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerManager : MonoBehaviour
{
   private Vector3 STARTSCENEPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 STARTMENUPOS = new Vector3(49.8f, 10.4f, -1.866f);
   private Vector3 GAMEOVERMENUPOS = new Vector3(54f, 10f, 1f);

   [SerializeField] private GameObject[] rayInteractor;
   [SerializeField] private GameObject[] teleportInteractor;
    [SerializeField] private GameObject PlayerHead;
    [SerializeField] private GameObject WaterheightPlane;
    
    [SerializeField] private float DROWNINGTIME = 10f;
    private float _timeUnderWater = 0f;
    public static event Action<GameManager.GameState> GameStateChangedPlayer;


   private void Awake()
   {
       GameManager.OnGameStateChanged +=HandleGameStateChanged;
   }

   private void OnDestroy()
   {
       GameManager.OnGameStateChanged -=HandleGameStateChanged;
   }

   private void Update()
   {
       if (Drawing())
       {
           Debug.Log($"[PlayerManager]: Player is drowning");
           GameStateChangedPlayer?.Invoke(GameManager.GameState.Drowned);
       };
   }

   public void SetPlayerPositionToStartGame()
    {
        transform.position = STARTSCENEPOS;
    }

    public void loadGamePlayScene()
    {
        DeactivateRayInteractor();
        ActivateTeleportInteractor();
        SetPlayerPositionToStartGame();
    }


    public void setPlayerPosToStartMenu()
    {
        transform.position = STARTMENUPOS;
    }

    private void DeactivateRayInteractor()
    {
        foreach (GameObject gameObject in rayInteractor)
        {
            gameObject.SetActive(false);
        }
    }

    private void ActivateRayInteractor()
    {
        foreach (GameObject gameObject in rayInteractor)
        {
            gameObject.SetActive(true);
        }
    }

    private void DeactivateTeleportInteractor()
    {
        foreach (GameObject gameObject in teleportInteractor)
        {
            gameObject.SetActive(false);
        }
    }

    private void ActivateTeleportInteractor()
    {
        foreach (GameObject gameObject in teleportInteractor)
        {
            gameObject.SetActive(true);
        }
    }

    public void SetPlayerPosToGameOverMenu()
    {
        transform.position = GAMEOVERMENUPOS;
    }
    
    public bool Drawing()
    {
        if (PlayerHead.transform.position.y < WaterheightPlane.transform.position.y)
        {
            _timeUnderWater += Time.deltaTime;
        }
        else
        {
            _timeUnderWater = 0;
        }

        return _timeUnderWater > DROWNINGTIME;
    }

    private void HandleGameStateChanged(GameManager.GameState gameState)
    {
        Debug.Log($"[PlayerManager]: GameState changed to {gameState}");
        if (gameState is GameManager.GameState.Drowned or GameManager.GameState.Win or GameManager.GameState.ElectricShock)
        {
            
            DeactivateTeleportInteractor();
            ActivateRayInteractor();
            SetPlayerPosToGameOverMenu();
        } else if (gameState == GameManager.GameState.Tutorial)
        {
           
            DeactivateRayInteractor();
           SetPlayerPositionToStartGame();
        }
        
    }
    
    
}
