using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerManager : MonoBehaviour
{
   private Vector3 STARTSCENEPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 STARTMENUPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 GAMEOVERMENUPOS = new Vector3(55.5f, 10f, 0f);

   [SerializeField] private GameObject[] rayInteractor;
   [SerializeField] private GameObject[] teleportInteractor;
    [SerializeField] private GameObject PlayerHead;
    [SerializeField] private GameObject WaterheightPlane;
    
    [SerializeField] private float DROWNINGTIME = 10f;
    private float timeUnderWater = 0f;
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
       if (drawing())
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
        deactivateRayInteractor();
        activateTeleportInteractor();
        SetPlayerPositionToStartGame();
    }


    public void setPlayerPosToStartMenu()
    {
        transform.position = STARTMENUPOS;
    }

    private void deactivateRayInteractor()
    {
        foreach (GameObject gameObject in rayInteractor)
        {
            gameObject.SetActive(false);
        }
    }

    private void activateRayInteractor()
    {
        foreach (GameObject gameObject in rayInteractor)
        {
            gameObject.SetActive(true);
        }
    }

    private void deactivateTeleportInteractor()
    {
        foreach (GameObject gameObject in teleportInteractor)
        {
            gameObject.SetActive(false);
        }
    }

    private void activateTeleportInteractor()
    {
        foreach (GameObject gameObject in teleportInteractor)
        {
            gameObject.SetActive(true);
        }
    }

    public void setPlayerPosToGameOverMenu()
    {
        transform.position = GAMEOVERMENUPOS;
    }
    
    public bool drawing()
    {
        if (PlayerHead.transform.position.y < WaterheightPlane.transform.position.y)
        {
            timeUnderWater += Time.deltaTime;
        }
        else
        {
            timeUnderWater = 0;
        }

        return timeUnderWater > DROWNINGTIME;
    }

    private void HandleGameStateChanged(GameManager.GameState gameState)
    {
        Debug.LogError($"[PlayerManager]: GameState changed to {gameState}");
        if (gameState is GameManager.GameState.Drowned or GameManager.GameState.Win or GameManager.GameState.ElectricShock)
        {
            
            deactivateTeleportInteractor();
            activateRayInteractor();
            setPlayerPosToGameOverMenu();
        } else if (gameState == GameManager.GameState.Tutorial)
        {
            Debug.LogError($"[PlayerManager]: GameState changed to {gameState}");
            deactivateRayInteractor();
           SetPlayerPositionToStartGame();
        }
        
    }
    
    
}
