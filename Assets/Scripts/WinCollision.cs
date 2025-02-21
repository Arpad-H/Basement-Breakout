using System;
using UnityEngine;

public class WinCollision : MonoBehaviour
{
    [SerializeField] private GameObject targetGameObject;
    private GameManager gameManager; 
    public static event Action<GameManager.GameState> GameStateChangedWinCollision;
    public static event Action<VoiceOverManager.Item> audioWonByDoorWin;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetGameObject)
        {
            Debug.Log($"[WinCollision[ {other.gameObject.name} || {targetGameObject.name}");
            GameStateChangedWinCollision?.Invoke(GameManager.GameState.Win);
            audioWonByDoorWin?.Invoke(VoiceOverManager.Item.WonByDoorWin);
            
        }

    }
}