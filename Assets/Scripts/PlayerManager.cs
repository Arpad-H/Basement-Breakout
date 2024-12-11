using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportPlayer : MonoBehaviour
{
   private Vector3 STARTSCENEPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 STARTMENUPOS = new Vector3(-2.364f, 3.97f, 8.326f);
   private Vector3 GAMEOVERMENUPOS = new Vector3(-2.364f, 3.97f, 8.326f);


   private void Awake()
   {
       GameManager.OnGameStateChanged +=OnGameStateChane;
   }

   private void OnDestroy()
   {
       throw new NotImplementedException();
   }

   public void setPlayerPositionToStartGame()
    {
        transform.position = STARTSCENEPOS;
    }


    public void setPlayerPosToStartMenu()
    {
        transform.position = STARTMENUPOS;
    }

    public void setPlayerPosToGameOverMenu()
    {
        transform.position = GAMEOVERMENUPOS;
    }

    private void OnGameStateChane(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Drowned || gameState == GameManager.GameState.Win ||
            gameState == GameManager.GameState.ElectricShock)
        {
            setPlayerPosToGameOverMenu();
        } else if (gameState == GameManager.GameState.Tutorial)
        {
           setPlayerPositionToStartGame();
        }
        {
            
        }
    }
    
    
}
