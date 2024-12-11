using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportPlayer : MonoBehaviour
{
   private Vector3 STARTSCENEPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 STARTMENUPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 GAMEOVERMENUPOS = new Vector3(55.5f, 10f, 0f);

   [SerializeField] private GameObject[] rayInteractor;
   [SerializeField] private GameObject[] teleportInteractor;


   private void Awake()
   {
       GameManager.OnGameStateChanged +=HandleGameStateChanged;
   }

   private void OnDestroy()
   {
       GameManager.OnGameStateChanged -=HandleGameStateChanged;
   }

   public void setPlayerPositionToStartGame()
    {
        transform.position = STARTSCENEPOS;
    }

    public void loadGamePlayScene()
    {
        deactivateRayInteractor();
        activateTeleportInteractor();
        setPlayerPositionToStartGame();
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

    private void HandleGameStateChanged(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Drowned || gameState == GameManager.GameState.Win ||
            gameState == GameManager.GameState.ElectricShock)
        {
            deactivateTeleportInteractor();
            activateRayInteractor();
            setPlayerPosToGameOverMenu();
        } else if (gameState == GameManager.GameState.Tutorial)
        {
            deactivateRayInteractor();
           setPlayerPositionToStartGame();
        }
        
    }
    
    
}
