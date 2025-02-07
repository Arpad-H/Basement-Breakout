using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGameStateOnCollsion : MonoBehaviour
{
   [SerializeField] private  GameObject targetGameObject;
   [SerializeField] private GameManager.GameState gameState;
   
   public static event Action<GameManager.GameState> GameStateChangedTriggerGameStateOnCollsion;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetGameObject)
        {
            GameStateChangedTriggerGameStateOnCollsion?.Invoke(gameState);
            Debug.Log($"[TriggerGameStateOnCollsion] {other.gameObject.name} collided with {targetGameObject.name}");
        }
    }
}
