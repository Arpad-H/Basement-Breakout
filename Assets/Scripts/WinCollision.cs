using System;
using UnityEngine;

public class WinCollision : MonoBehaviour
{
    [SerializeField] private GameObject targetGameObject;
    private GameManager gameManager; 
    public static event Action<GameManager.GameState> GameStateChangedWinCollision;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetGameObject)
        {
            Debug.Log($"[WinCollision[ {other.gameObject.name} || {targetGameObject.name}");
            GameStateChangedWinCollision?.Invoke(GameManager.GameState.Win);
            
        }

    }
}